using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using NpcChat.Backend.Interfaces;
using NpcChat.Util;
using NpcChat.ViewModels.Panels.ScriptEditor.TextBlockElements;
using NpcChatSystem;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Identifiers;
using NpcChatSystem.System.TypeStore.Stores;
using NpcChatSystem.Utilities;
using Prism.Commands;
using SpellCheck = NpcChat.Backend.Validation.SpellCheck;

namespace NpcChat.ViewModels.Panels.ScriptEditor
{
    /// <summary>
    /// View model for a line of character dialog from a single person
    /// </summary>
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

                if (m_editMode == EditMode.TextBlock)
                    PrepairTextBlockEdit();
            }
        }

        /// <summary>
        /// Id of the character that says this segment of text
        /// </summary>
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

        /// <summary>
        /// Parts of text shown when using <see cref="TextBlock"/> to edit text
        /// </summary>
        public FlowDocument DialogDocument
        {
            get => m_dialogDocument;
            set
            {
                m_dialogDocument = value;
                RaisePropertyChanged(nameof(DialogDocument));
            }
        }

        /// <summary>
        /// Is this dialog segment currently being inspected by the user?
        /// </summary>
        public bool InspectionActive
        {
            get => m_inspectionActive;
            set
            {
                if (m_inspectionActive == value) return;

                m_inspectionActive = value;
                RaisePropertyChanged();

                if(InspectionActive && TextEditMode)
                {
                    if(SpellCheck.ContainsSpellingSuggestion(DialogSegment.SegmentParts))
                    {
                        // update any existing suggestions
                        PrepairTextBlockEdit();
                    }
                }
            }
        }

        /// <summary>
        /// Style of editing that is being performed
        /// </summary>
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

                if (m_editMode == EditMode.TextBlock)
                    PrepairTextBlockEdit();
            }
        }

        /// <summary>
        /// Is element style text edit mode active?
        /// </summary>
        public bool ElementEditMode => EditMode == EditMode.Elements;

        /// <summary>
        /// Is Text Block style edit mode active?
        /// </summary>
        public bool TextEditMode => EditMode == EditMode.TextBlock;

        public IReadOnlyList<string> DialogElementTypes => DialogTypeStore.Dialogs;
        public ICommand AddDialogElementCommand => m_addDialogElement;
        public ICommand RemoveDialogElementCommand => m_removeDialogElement;
        public ICommand DestroyCommand { get; }

        private bool m_inspectionActive = false;
        private DialogSegment m_dialogSegment = null;
        private DelegateCommand<string> m_addDialogElement;
        private DelegateCommand<IDialogElement> m_removeDialogElement;
        private EditMode m_editMode;
        private string[] m_textBlockElements = new string[0];
        private FlowDocument m_dialogDocument;

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

        /// <summary>
        /// Combine all text elements into a single piece of text for showing multiple text elements as a single block of text
        /// </summary>
        /// <remarks>
        /// This add context aware highlighting to text
        /// </remarks>
        private void PrepairTextBlockEdit()
        {
            FlowDocument dialogs = new FlowDocument();
            Paragraph paragraph = new Paragraph();

            foreach (IDialogElement part in DialogSegment.SegmentParts)
            {
                if (part.AllowsInspection)
                {
                    foreach (Inline inline in SpellCheck.CheckElement(part))
                    {
                        paragraph.Inlines.Add(inline);
                    }
                }
                else
                {
                    paragraph.Inlines.Add(new Run(part.Text));
                }
            }

            dialogs.Blocks.Add(paragraph);
            DialogDocument = dialogs;
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

            if (m_editMode == EditMode.TextBlock)
                PrepairTextBlockEdit();
        }
    }
}
