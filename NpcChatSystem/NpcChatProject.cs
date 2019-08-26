using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using NpcChatSystem.Data;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.System;

namespace NpcChatSystem
{
    public class NpcChatProject
    {
        /// <summary>
        /// Character store for the last loaded project, null if no project has been loaded
        /// </summary>
        public static CharacterStore Characters { get; private set; }
        /// <summary>
        /// Dialog Manager for the last loaded project, null if no project has been loaded
        /// </summary>
        public static DialogManager Dialogs { get; private set; }
        /// <summary>
        /// Story Element Manager for the last loaded project, null if no project has been loaded
        /// </summary>
        public static StoryElementManager StoryElements { get; private set; }
        /// <summary>
        /// Last project loaded, if another project is loaded this is replaced
        /// </summary>
        public static NpcChatProject LastProject { get; private set; }


        /// <summary>
        /// Character store for the project
        /// </summary>
        public CharacterStore ProjectCharacters { get; }
        /// <summary>
        /// Dialog Manager store for the project
        /// </summary>
        public DialogManager ProjectDialogs { get; }
        /// <summary>
        /// Story Element Manager store for the project
        /// </summary>
        public StoryElementManager ProjectStoryElements { get; }

        /// <summary>
        /// Create a new Project
        /// </summary>
        public NpcChatProject()
        {
            Characters = ProjectCharacters = new CharacterStore();
            Dialogs = ProjectDialogs = new DialogManager();
            StoryElements = ProjectStoryElements = new StoryElementManager();
            LastProject = this;
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