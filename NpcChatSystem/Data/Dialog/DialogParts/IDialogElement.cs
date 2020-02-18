using System.ComponentModel;

namespace NpcChatSystem.Data.Dialog.DialogParts
{
    /// <summary>
    /// Represents a single element from a piece of dialog, like text or information about a particular element which is filled in dynamically
    /// </summary>
    public interface IDialogElement : INotifyPropertyChanged
    {
        /// <summary>
        /// Text representation of this element
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Name of the IDialogElement implementation type
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// Does this element allow for validation (such as spell check)
        /// </summary>
        bool AllowsInspection { get; }

        /// <summary>
        /// Change <see cref="Text"/> as described by <paramref name="source"/> to <paramref name="edit"/>
        /// </summary>
        /// <param name="source">Original section of text that should be changed</param>
        /// <param name="edit">Text that <paramref name="source"/> should become</param>
        /// <returns>true if operation was successful</returns>
        bool IntegrateCorrection(string source, string edit);
    }
}
