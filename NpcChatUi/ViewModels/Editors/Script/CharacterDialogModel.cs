using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NpcChat.Util;
using NpcChatSystem.Data;

namespace NpcChat.ViewModels.Editors.Script
{
    public class CharacterDialogModel : NotificationObject
    {
        public DialogSegment DialogSegment
        {
            get => dialogSegment;
            set
            {
                dialogSegment = value;
                RaisePropertyChanged();
            }
        }

        public int DialogId
        {
            get => dialogSegment?.DialogId ?? -1;
            set => RetrieveDialog(value);
        }


        private DialogSegment dialogSegment = null;

        public CharacterDialogModel()
        {

        }

        private void RetrieveDialog(int dialogId)
        {

        }
    }
}
