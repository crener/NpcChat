using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.System.TypeStore;
using NUnit.Framework;

namespace NpcChatTest.System.TypeStore
{
    public class MasterTypeStoreTests
    {
        [Test]
        public void AddAssemblyNull()
        {
            Assert.DoesNotThrow(() =>
            {
                MasterTypeStore.AddAssembly(null);
            });
        }
    }
}
