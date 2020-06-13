using NodeNetwork.ViewModels;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Layout
{
    public interface ILayout
    {
        /// <summary>
        /// Amount of items along the X axis
        /// </summary>
        int Columns { get; }

        /// <summary>
        /// Lowest column value (inclusive)
        /// </summary>
        int ColumnMin { get; }

        /// <summary>
        /// Amount of items along the Y axis
        /// </summary>
        int Rows { get; }

        /// <summary>
        /// Lowest row value (inclusive)
        /// </summary>
        int RowMin { get; }

        /// <summary>
        /// Proposed layout of the node network on a 2D plane from the last <see cref="Layout"/> call
        /// </summary>
        NodeViewModel this[int x, int y] { get; }

        void Layout(NetworkViewModel network);
    }
}