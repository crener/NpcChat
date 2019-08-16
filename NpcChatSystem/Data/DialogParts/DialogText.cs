using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.Data.DialogParts
{
    /// <summary>
    /// Piece of text
    /// </summary>
    [DebuggerDisplay("{Text}")]
    class DialogText : IDialogElement
    {
        public string Text { get; set; } = "";
    }
}
