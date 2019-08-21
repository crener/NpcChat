using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data;
using NpcChatSystem.Data.Dialog;

namespace NpcChat.ViewModels.Editors.Script.Util
{
    /// <summary>
    /// wrapper for the dialog so the xaml can bind to a dedicated user control
    /// </summary>
    public class DialogWrapper
    {
        public DialogSegment Dialog { get; set; }

        public DialogWrapper(DialogSegment dialog)
        {
            Dialog = dialog;
        }
    }
}
