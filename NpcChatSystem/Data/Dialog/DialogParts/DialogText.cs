﻿using System.Diagnostics;

namespace NpcChatSystem.Data.Dialog.DialogParts
{
    /// <summary>
    /// Piece of text
    /// </summary>
    [DebuggerDisplay("{Text}")]
    public class DialogText : IDialogElement
    {
        public string Text { get; set; } = "";
    }
}
