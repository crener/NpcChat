using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NpcChat.ViewModels.Editors.Script;
using NpcChatSystem.Data;

namespace NpcChat.Views.Editors.Script
{
    /// <summary>
    /// Interaction logic for ScriptCharDialog.xaml
    /// </summary>
    public partial class CharacterDialog : UserControl
    {
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register(nameof(Editable), typeof(bool),
            typeof(CharacterDialog), new PropertyMetadata(false));
        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set => SetValue(EditableProperty, value);
        }

        /*public static readonly DependencyProperty DialogIdProperty = DependencyProperty.Register(nameof(DialogId), typeof(int),
            typeof(CharacterDialog), new PropertyMetadata(-1, DialogIdChanged));
        private static void DialogIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue) return;

            CharacterDialog control = d as CharacterDialog;
            control.Model.DialogId = (int)e.NewValue;
        }

        public int DialogId
        {
            get => (int)GetValue(DialogIdProperty);
            set => SetValue(EditableProperty, value);
        }*/

        protected CharacterDialogModel Model { get; }

        public CharacterDialog()
        {
            //Model = new CharacterDialogModel();
            InitializeComponent();
        }
    }
}
