using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NpcChatSystem.Annotations;
using NotImplementedException = System.NotImplementedException;

namespace NpcChatSystem.Data.Dialog.DialogParts
{
    /// <summary>
    /// Piece of text
    /// </summary>
    [DebuggerDisplay("{Text}")]
    public class DialogText : IDialogElement, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get => m_text;
            set
            {
                m_text = value;
                RaiseChanged();
            }
        }


        private string m_text = "";

        [NotifyPropertyChangedInvocator]
        protected virtual void RaiseChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
