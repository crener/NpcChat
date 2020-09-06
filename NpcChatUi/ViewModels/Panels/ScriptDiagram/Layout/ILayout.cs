using NodeNetwork.ViewModels;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Layout
{
    public interface ILayout
    {
        /// <summary>
        /// Orders the nodes according to the layout type
        /// </summary>
        /// <param name="network">node network to sort</param>
        void Layout(NetworkViewModel network);
    }
}