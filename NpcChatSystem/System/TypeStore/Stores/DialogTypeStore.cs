using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Utilities;

namespace NpcChatSystem.System.TypeStore.Stores
{
    [Export(typeof(ITypeStore))]
    public class DialogTypeStore : BaseTypeStore<IDialogElement>
    {
        public static IReadOnlyList<string> Dialogs => Instance.m_elementNames;

        public static DialogTypeStore Instance { get; }

        static DialogTypeStore()
        {
            Instance = new DialogTypeStore();
        }

        /// <summary>
        /// Checks that the IDialogElement type can be created via reflection
        /// </summary>
        /// <param name="type">IDialogElement type</param>
        /// <returns>true if type is value</returns>
        protected override bool ValidateConstructors(TypeInfo type)
        {
            bool emptyConstructor = false, projectConstructor = false;
            foreach (ConstructorInfo constructor in type.DeclaredConstructors)
            {
                if (!constructor.IsPublic) continue;

                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                    emptyConstructor = true;
                else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(NpcChatProject))
                    projectConstructor = true;
                else
                {
                    bool defaultTypes = true;
                    bool containsProject = false;
                    foreach (ParameterInfo param in parameters)
                    {
                        if (param.ParameterType == typeof(NpcChatProject))
                        {
                            if (!containsProject) containsProject = true;
                            else
                            {
                                //todo add warning log here once log library is in place
                                //todo add test case for an element with 2 npc project parameters as this is a strange edge case...

                                //a valid constructor should only require one npcProject
                                containsProject = false;
                                break;
                            }
                        }
                        else if (!param.HasDefaultValue) defaultTypes = false;
                    }

                    if (containsProject && defaultTypes)
                    {
                        projectConstructor = true;
                    }
                }
            }

            if (emptyConstructor || projectConstructor) return true;

            string msg =
                $"Failed to add Dialog Element type '{type.FullName}' as it does not have any supported constructors! At least one of the following public constructors must be implemented:" +
                $"    public {type.Name}()..." +
                $"    public {type.Name}({nameof(NpcChatProject)} project)..." +
                $"Variation of this is possible but all none '{nameof(NpcChatProject)}' parameters must have a default value!";
            Logging.Logger.Error(msg);

            return false;
        }

        public override IDialogElement CreateEntity(string elementName, NpcChatProject project = null)
        {
            if (m_elementLookup.ContainsKey(elementName))
            {
                Type type = m_elementLookup[elementName];

                if (type.GetConstructors().Any(c => c.GetParameters().Length == 0))
                {
                    return Activator.CreateInstance(type) as IDialogElement;
                }
                else
                {
                    foreach (ConstructorInfo constructor in type.GetConstructors())
                    {
                        if (!constructor.IsPublic) continue;

                        ParameterInfo[] parameters = constructor.GetParameters();
                        List<object> constructorParameters = new List<object>(parameters.Length);

                        bool defaultTypes = true;
                        bool containsProject = false;
                        foreach (ParameterInfo param in parameters)
                        {
                            if (param.ParameterType == typeof(NpcChatProject))
                            {
                                if (!containsProject)
                                {
                                    constructorParameters.Add(project);
                                    containsProject = true;
                                }
                                else
                                {
                                    containsProject = false;
                                    break;
                                }
                            }
                            else if (!param.HasDefaultValue)
                            {
                                defaultTypes = false;
                                break;
                            }
                            else
                            {
                                constructorParameters.Add(param.DefaultValue);
                            }
                        }

                        if (defaultTypes && containsProject)
                        {
                            return constructor.Invoke(constructorParameters.ToArray()) as IDialogElement;
                        }
                    }
                }

                return Activator.CreateInstance(type, project) as IDialogElement;
            }

            Logging.Logger.Warn($"Unable to instantiate '{elementName}'");
            return null;
        }

        public override IDialogElement CreateEntity(Type elementType, NpcChatProject project = null)
        {
            if (!m_elementLookup.ContainsValue(elementType)) return null;

            KeyValuePair<string, Type> key = m_elementLookup.First(k => k.Value == elementType);
            return CreateEntity(key.Key, project);
        }
    }
}
