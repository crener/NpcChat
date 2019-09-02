using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using NpcChat.Properties;

namespace NpcChat.Util
{
    public class DependencyNotificationObject : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
