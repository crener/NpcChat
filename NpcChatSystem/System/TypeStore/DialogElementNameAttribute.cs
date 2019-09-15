﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using NpcChatSystem.Data.Dialog.DialogParts;

namespace NpcChatSystem.System.TypeStore
{
    /// <summary>
    /// Data attribute used to markup <see cref="IDialogElement"/> for <see cref="DialogTypeStore"/> to use when showing a list of all possible dialog options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DialogElementNameAttribute : Attribute
    {
        public string Name { get; }

        public DialogElementNameAttribute(string name)
        {
            Name = name;
        }
    }
}
