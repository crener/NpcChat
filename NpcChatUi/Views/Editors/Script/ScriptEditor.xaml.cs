using System.Windows;
using System.Windows.Controls;
using NpcChat.ViewModels.Editors.Script;

namespace NpcChat.Views.Editors.Script
{
    /// <summary>
    /// Interaction logic for ScriptEditor.xaml
    /// </summary>
    public partial class ScriptEditor : UserControl
    {
        private ScriptEditorModel model;

        public ScriptEditor()
        {
            InitializeComponent();
            DataContext = model = new ScriptEditorModel();
        }


        public static readonly DependencyProperty DialogTreeIdProperty = DependencyProperty.Register(nameof(DialogTreeId), typeof(int),
            typeof(ScriptEditor), new PropertyMetadata(-1, DialogMode));
        private static void DialogMode(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;

            ScriptEditor control = d as ScriptEditor;
            control.model.DialogTree = (int)e.NewValue;
        }
        public int DialogTreeId
        {
            get => (int)GetValue(DialogTreeIdProperty);
            set => SetValue(DialogTreeIdProperty, value);
        }
    }
}
