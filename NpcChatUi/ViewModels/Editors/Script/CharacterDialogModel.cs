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

namespace NpcChat.ViewModels.Editors.Script
{
    public class CharacterDialogModel : NotificationObject
    {
        public NpcChatProject Project { get; set; }
        public DialogSegment DialogSegment
        {
            get => m_dialogSegment;
            set
            {
                m_dialogSegment = value;
                RaisePropertyChanged();
            }
        }

        public int DialogSegmentId
        {
            get => m_dialogSegment?.Id.DialogSegmentId ?? -1;
            set => RetrieveDialog(value);
        }

        public int TreeId
        {
            get => m_dialogSegment?.Id.DialogTreeId ?? -1;
            set
            {
                RetrieveDialog(value);
            }
        }


        private DialogSegment m_dialogSegment = null;


        public CharacterDialogModel(NpcChatProject project, DialogSegment dialog)
        {
            Project = project;
            m_dialogSegment = dialog;
        }

        private void RetrieveDialog(int dialogId)
        {
            if (Project == null) return;

            DialogTree tree = Project.ProjectDialogs.GetDialog(dialogId);
        }
    }
}
