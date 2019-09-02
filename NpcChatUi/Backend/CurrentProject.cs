using NpcChatSystem;
using NpcChatSystem.System;

namespace NpcChat.Backend
{
    public static class CurrentProject
    {
        /// <summary>
        /// Character store for the last loaded project, null if no project has been loaded
        /// </summary>
        public static CharacterStore Characters => Project?.ProjectCharacters;
        /// <summary>
        /// Dialog Manager for the last loaded project, null if no project has been loaded
        /// </summary>
        public static DialogManager Dialogs => Project?.ProjectDialogs;
        /// <summary>
        /// Story Element Manager for the last loaded project, null if no project has been loaded
        /// </summary>
        public static StoryElementManager StoryElements => Project?.ProjectStoryElements;
        /// <summary>
        /// Last project loaded, if another project is loaded this is replaced
        /// </summary>
        public static NpcChatProject Project { get; set; }
    }
}
