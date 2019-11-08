using System.Windows.Controls;
using NpcChat.ViewModels.Panels.Tree.Nodes;

namespace NpcChat.Views.Panels.Tree.Nodes
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