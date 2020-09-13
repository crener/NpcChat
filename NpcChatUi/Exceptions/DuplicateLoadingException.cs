using System;
using System.IO;

namespace NpcChat.Exceptions
{
    /// <summary>
    /// Exception thrown when loading an item twice from file when it should have been loaded once
    /// </summary>
    public class DuplicateLoadingException : IOException
    {
        public DuplicateLoadingException() { }
        public DuplicateLoadingException(string message) : base(message) { }
    }
}