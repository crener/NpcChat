using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Annotations;

namespace NpcChatSystem.Data.Util
{
    public class ProjectNotificationObject : ProjectObject, INotifyPropertyChanged
    {
        public ProjectNotificationObject(NpcChatProject project) 
            : base(project)
        {

        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaiseChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
