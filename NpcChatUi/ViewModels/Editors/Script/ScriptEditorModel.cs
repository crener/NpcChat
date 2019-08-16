using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Media3D;
using NpcChat.Util;
using NpcChat.ViewModels.Editors.Script.Util;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.DialogTreeItems;

namespace NpcChat.ViewModels.Editors.Script
{
    public class ScriptEditorModel : NotificationObject
    {
        public ObservableCollection<DialogSegment> Speech
        {
            get => m_speech;
            set
            {
                m_speech = value;
                RaisePropertyChanged();
            }
        }

        public int DialogTree
        {
            get => m_tree?.Id ?? 0;
            set => SetDialogTree(value);
        }

        private ObservableCollection<DialogSegment> m_speech;
        private DialogTree m_tree;

        public ScriptEditorModel()
        {
            m_speech = new ObservableCollection<DialogSegment>();
        }

        public void SetDialogTree(int dialogTreeId)
        {
            m_tree = NpcChatProject.Dialogs.GetDialog(dialogTreeId);
            TreePart part = m_tree.GetStart();

            Speech.Clear();
            Speech.AddRange(part.Dialog);
        }
    }
}