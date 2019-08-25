using System.Windows.Controls;
using NpcChat.ViewModels.Editors.Tree.Nodes;

namespace NpcChat.Views.Editors.Tree.Nodes
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