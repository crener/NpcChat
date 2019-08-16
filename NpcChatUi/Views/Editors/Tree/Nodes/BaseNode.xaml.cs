using System.Windows.Controls;
using NpcChat.ViewModels;
using NpcChat.ViewModels.Nodes;

namespace NpcChat.Views.Nodes
{
    public partial class BaseNode : UserControl
    {
        public BaseNode()
        {
            DataContext = new BaseNodeModel();
            InitializeComponent();
        }
    }
}