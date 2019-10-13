using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using NpcChatSystem.Utilities;

namespace NpcChatSystem.System.TypeStore
{
    public static class MasterTypeStore
    {
        private static List<ITypeStore> m_store = new List<ITypeStore>();
        private static HashSet<Assembly> m_assemblies = new HashSet<Assembly>();
        //private static CompositionContainer m_container;
        //private static AggregateCatalog m_catalog;

        static MasterTypeStore()
        {
            try
            {
                //m_catalog = new AggregateCatalog();

                {
                    Assembly assembly = typeof(MasterTypeStore).Assembly;
                    //m_catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                    m_assemblies.Add(assembly);
                }

                //m_container = new CompositionContainer(m_catalog);
            }
            catch (Exception ex)
            {
                Logging.Logger.Error(ex);
            }
        }

        /// <summary>
        /// adds an assembly for indexing into the type stores
        /// </summary>
        /// <param name="assembly">assembly to add</param>
        internal static void AddAssembly(Assembly assembly)
        {
            if (assembly == null) return;
            if (m_assemblies.Contains(assembly)) return;

            m_assemblies.Add(assembly);
            foreach(ITypeStore store in m_store)
            {
                store.ScanAssembly(assembly);
            }
        }

        internal static void RegisterTypeStore(ITypeStore store)
        {
            //check if the assembly has already been added
            Assembly checkAssembly = store.GetType().Assembly;
            if(!m_assemblies.Contains(checkAssembly))
            {
                //m_catalog.Catalogs.Add(new AssemblyCatalog(checkAssembly));
                m_assemblies.Add(checkAssembly);
                foreach (ITypeStore typeStore in m_store)
                {
                    typeStore.ScanAssembly(checkAssembly);
                }
            }

            m_store.Add(store);

            //make sure to add all assemblies to the type store
            foreach(Assembly assembly in m_assemblies)
            {
                store.ScanAssembly(assembly);
            }
        }
    }
}
