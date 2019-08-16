using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using NpcChatSystem.Data;
using NpcChatSystem.System;

namespace NpcChatSystem
{
    public class NpcChatProject
    {
        public static CharacterStore Characters { get; private set; }
        public static DialogManager Dialogs { get; private set; }
        public static StoryElementManager StoryElements { get; private set; }

        public CharacterStore ProjectCharacters { get; private set; }
        public DialogManager ProjectDialogs { get; private set; }
        public StoryElementManager ProjectStoryElements { get; private set; }

        /// <summary>
        /// Create a new Project
        /// </summary>
        public NpcChatProject()
        {
            Characters = ProjectCharacters = new CharacterStore();
            Dialogs = ProjectDialogs = new DialogManager();
            StoryElements = ProjectStoryElements = new StoryElementManager();
        }

        /// <summary>
        /// Load a project from an existing source
        /// </summary>
        /// <param name="path">path to project source</param>
        public NpcChatProject(string path) : this()
        {
            Characters.RegisterNewCharacter(out int _, new Character("Fiona"));
        }
    }
}