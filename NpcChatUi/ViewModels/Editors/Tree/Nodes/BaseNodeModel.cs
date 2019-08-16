using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using NpcChat.Annotations;
using NpcChat.Util;

namespace NpcChat.ViewModels.Nodes
{
    public class BaseNodeModel : NotificationObject
    {
        public bool SingleLink
        {
            get => singleLink;
            set
            {
                singleLink = value;
                RaisePropertyChanged();
            }
        }
        
        public Brush BackGroundColour
        {
            get => backGroundColour;
            protected set
            {
                backGroundColour = value;
                RaisePropertyChanged();
            }
        }

        public string Title
        {
            get => titleText;
            set
            {
                titleText = value + " 1";
                RaisePropertyChanged();
            }
        }

        private Brush backGroundColour = new SolidColorBrush(Colors.Red);
        private bool singleLink;
        private string titleText = "bleh";

        public BaseNodeModel()
        {
            
        }
    }
}