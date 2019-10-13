using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using NpcChatSystem.Branching.EvaluationContainers;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Utilities;

namespace NpcChatSystem.System.TypeStore.Stores
{
    [Export(typeof(ITypeStore))]
    public class EvaluationContainerTypeStore : BaseTypeStore<IEvaluationContainer>
    {
        public static IReadOnlyList<string> ContainerNames => Instance.m_elementNames;

        public static EvaluationContainerTypeStore Instance { get; }

        static EvaluationContainerTypeStore()
        {
            Instance = new EvaluationContainerTypeStore();
        }

        /// <summary>
        /// Checks that the IDialogElement type can be created via reflection
        /// </summary>
        /// <param name="type">IDialogElement type</param>
        /// <returns>true if type is value</returns>
        protected override bool ValidateConstructors(TypeInfo type)
        {
            foreach (ConstructorInfo constructor in type.DeclaredConstructors)
            {
                if (!constructor.IsPublic) continue;

                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                    return true;
            }

            Logging.Logger.Error($"Failed to add Evaluation Container type '{type.FullName}' as it does not have any empty constructors!");
            return false;
        }

        public override IEvaluationContainer CreateEntity(string elementName, NpcChatProject project = null)
        {
            if (m_elementLookup.ContainsKey(elementName))
            {
                Type type = m_elementLookup[elementName];

                if (type.GetConstructors().Any(c => c.GetParameters().Length == 0))
                {
                    return Activator.CreateInstance(type) as IEvaluationContainer;
                }
            }

            Logging.Logger.Warn($"Unable to instantiate '{elementName}'");
            return null;
        }

        public override IEvaluationContainer CreateEntity(Type elementType, NpcChatProject project = null)
        {
            if (!m_elementLookup.ContainsValue(elementType)) return null;

            KeyValuePair<string, Type> key = m_elementLookup.First(k => k.Value == elementType);
            return CreateEntity(key.Key);
        }
    }
}
