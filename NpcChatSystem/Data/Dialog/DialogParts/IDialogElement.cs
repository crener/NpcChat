namespace NpcChatSystem.Data.Dialog.DialogParts
{
    /// <summary>
    /// Represents a single element from a piece of dialog, like text or information about a particular element which is filled in dynamically
    /// </summary>
    public interface IDialogElement
    {
        string Text { get; }
    }
}
