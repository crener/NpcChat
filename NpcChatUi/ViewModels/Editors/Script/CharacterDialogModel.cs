using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;

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
                m_dialogSegment = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DialogSegmentId));
            }
        }

        public DialogSegmentIdentifier DialogSegmentId
        {
            get => m_dialogSegment?.Id ?? null;
            set => RetrieveDialog(value);
        }

        private DialogSegment m_dialogSegment = null;

        public CharacterDialogModel(NpcChatProject project, DialogSegment dialog)
        {
            Project = project;
            m_dialogSegment = dialog;
        }

        private void RetrieveDialog(DialogSegmentIdentifier dialogId)
        {
            DialogSegment tree = Project?.ProjectDialogs[dialogId];
            if(tree == null) return;

            m_dialogSegment = tree;
        }
    }
}
