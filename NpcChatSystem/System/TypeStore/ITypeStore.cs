using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.System.TypeStore
{
    public interface ITypeStore
    {
        void ScanAssembly(Assembly assembly);
    }
}
