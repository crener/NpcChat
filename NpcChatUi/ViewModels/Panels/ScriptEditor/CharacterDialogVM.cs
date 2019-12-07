using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using NpcChat.Backend.Interfaces;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Identifiers;
using NpcChatSystem.System.TypeStore.Stores;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Panels.ScriptEditor
{
    public class CharacterDialogVM : NotificationObject
    {
        public NpcChatProject Project { get; }
        public DialogSegment DialogSegment
        {
            get => m_dialogSegment;
            set
            {
                if (DialogSegment != null) DialogSegment.PropertyChanged -= DialogChanged;
                m_dialogSegment = value;
                if (DialogSegment != null) DialogSegment.PropertyChanged += DialogChanged;

                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DialogSegmentId));
            }
        }

        public int CharacterId
        {
            get => m_dialogSegment?.CharacterId ?? 0;
            set
            {
                m_dialogSegment.CharacterId = value;
                RaisePropertyChanged();
            }
        }

        public DialogSegmentIdentifier DialogSegmentId
        {
            get => m_dialogSegment?.Id ?? null;
            set => RetrieveDialog(value);
        }

        public EditMode EditMode
        {
            get => m_editMode;
            set
            {
                if (m_editMode == value) return;

                m_editMode = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ElementEditMode));
                RaisePropertyChanged(nameof(TextEditMode));
            }
        }

        public bool ElementEditMode => EditMode == EditMode.Elements;
        public bool TextEditMode => EditMode == EditMode.TextBlock;

        public IReadOnlyList<string> DialogElementTypes => DialogTypeStore.Dialogs;
        public ICommand AddDialogElementCommand => m_addDialogElement;
        public ICommand RemoveDialogElementCommand => m_removeDialogElement;
        public ICommand DestroyCommand { get; }

        private DialogSegment m_dialogSegment = null;
        private DelegateCommand<string> m_addDialogElement;
        private DelegateCommand<IDialogElement> m_removeDialogElement;
        private EditMode m_editMode;

        public CharacterDialogVM(NpcChatProject project, [NotNull] DialogSegment dialog)
        {
            Project = project;
            DialogSegment = dialog;

            m_addDialogElement = new DelegateCommand<string>(AddDialogElement);
            m_removeDialogElement = new DelegateCommand<IDialogElement>(RemoveDialogElement);
            DestroyCommand = new DelegateCommand(DestroyCharacterDialog);
        }

        private void AddDialogElement(string dialogElementName)
        {
            IDialogElement element = DialogTypeStore.Instance.CreateEntity(dialogElementName, Project);

            if (element != null) DialogSegment.AddDialogElement(element);
        }

        private void RemoveDialogElement(IDialogElement element)
        {
            if (element == null) return;
            DialogSegment.RemoveDialogElement(element);
        }

        private void RetrieveDialog(DialogSegmentIdentifier dialogId)
        {
            DialogSegment tree = Project?.ProjectDialogs[dialogId];
            if (tree == null) return;

            DialogSegment = tree;
        }

        private void DestroyCharacterDialog()
        {
            DialogTreeBranch branch = Project[(DialogTreeBranchIdentifier)DialogSegmentId];
            if (branch == null)
            {
                Logging.Logger.Warn($"Failed to remove dialog '{DialogSegmentId}' due to missing branch!");
                return;
            }

            bool success = branch.RemoveDialog(DialogSegmentId);
            if (!success)
            {
                Logging.Logger.Warn($"Failed to remove dialog '{DialogSegmentId}' from branch '{branch.Id}'");
            }
        }

        private void DialogChanged(object s, PropertyChangedEventArgs a)
        {
            RaisePropertyChanged(nameof(DialogSegment));
            RaisePropertyChanged(nameof(DialogSegmentId));
        }
    }
}
