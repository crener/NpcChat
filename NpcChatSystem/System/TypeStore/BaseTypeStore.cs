using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Utilities;

namespace NpcChatSystem.System.TypeStore
{
    public abstract class BaseTypeStore<T> : ITypeStore
    {
        protected List<string> m_elementNames = new List<string>();
        protected Dictionary<string, Type> m_elementLookup = new Dictionary<string, Type>();

        protected BaseTypeStore()
        {
            MasterTypeStore.RegisterTypeStore(this);
        }

        public void ScanAssembly(Assembly ass)
        {
            foreach (TypeInfo type in ass.DefinedTypes.Where(t => t.ImplementedInterfaces.Contains(typeof(T))))
            {
                if (type.ImplementedInterfaces.Contains(typeof(T)))
                {
                    if (ValidateConstructors(type))
                    {
                        string name = type.Name;

                        if (type.CustomAttributes.Any(a => a.AttributeType == typeof(NiceTypeNameAttribute)))
                        {
                            NiceTypeNameAttribute dialog = type.GetCustomAttribute<NiceTypeNameAttribute>();
                            name = dialog.Name;
                        }

                        m_elementNames.Add(name);
                        m_elementLookup.Add(name, type);
                    }
                }
            }
        }

        protected abstract bool ValidateConstructors(TypeInfo type);

        public abstract T CreateEntity(string elementName, NpcChatProject project = null);

        public abstract T CreateEntity(Type elementType, NpcChatProject project = null);

        public TE CreateEntity<TE>(NpcChatProject project = null) where TE : class, T
        {
            Type desiredType = typeof(TE);
            if (!m_elementLookup.ContainsValue(desiredType))
            {
                Logging.Logger.Warn($"Unable to instantiate '{typeof(TE).FullName}'");
                return null;
            }

            KeyValuePair<string, Type> key = m_elementLookup.First(k => k.Value == desiredType);
            return CreateEntity(key.Key, project) as TE;
        }

        public TE CreateEntity<TE>(string elementName, NpcChatProject project = null) where TE : class, T
        {
            return CreateEntity(elementName, project) as TE;
        }
    }
}
