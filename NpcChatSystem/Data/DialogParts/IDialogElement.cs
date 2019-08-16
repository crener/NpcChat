using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.Data.DialogParts
{
    /// <summary>
    /// Represents a single element from a piece of dialog, like text or information about a particular element which is filled in dynamically
    /// </summary>
    interface IDialogElement
    {
        string Text { get; }
    }
}
