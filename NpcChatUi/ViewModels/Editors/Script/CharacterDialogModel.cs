using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Input;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using NpcChatSystem.System.TypeStore;
using Prism.Commands;

namespace NpcChat.ViewModels.Editors.Script
{
    public class CharacterDialogModel : NotificationObject
    {
        public NpcChatProject Project { get; }
        public DialogSegment DialogSegment
        {
            get => m_dialogSegment;
            set
            {
                if(DialogSegment != null) DialogSegment.PropertyChanged -= DialogChanged;
                m_dialogSegment = value;
                if (DialogSegment != null) DialogSegment.PropertyChanged += DialogChanged;

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DialogSegmentId));
            }
        }

        public int CharacterId
        {
            get => m_dialogSegment?.CharacterId ?? 0;
            set => m_dialogSegment.CharacterId = value;
        }

        public DialogSegmentIdentifier DialogSegmentId
        {
            get => m_dialogSegment?.Id ?? null;
            set => RetrieveDialog(value);
        }

        public IReadOnlyList<string> DialogElementTypes => DialogTypeStore.Dialogs;

        public ICommand AddDialogElementCommand => m_addDialogElement;

        private DialogSegment m_dialogSegment = null;
        private DelegateCommand<string> m_addDialogElement;

        public CharacterDialogModel(NpcChatProject project, DialogSegment dialog)
        {
            Project = project;
            DialogSegment = dialog;

            m_addDialogElement = new DelegateCommand<string>(AddDialogElement);
        }

        private void AddDialogElement(string dialogElementName)
        {
            IDialogElement element = DialogTypeStore.CreateDialogElement(dialogElementName, Project);

            if (element != null) DialogSegment.AddDialogElement(element);
        }

        private void RetrieveDialog(DialogSegmentIdentifier dialogId)
        {
            DialogSegment tree = Project?.ProjectDialogs[dialogId];
            if (tree == null) return;

            DialogSegment = tree;
        }

        private void DialogChanged(object s, PropertyChangedEventArgs a)
        {
            RaisePropertyChanged(nameof(DialogSegment));
            //RaisePropertyChanged(nameof(DialogSegmentId));
        }
    }
}
