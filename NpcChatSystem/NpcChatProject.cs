using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using NpcChatSystem.Data;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.System;

namespace NpcChatSystem
{
    public class NpcChatProject
    {
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

        public const string ProjectExtension = "npcProject";

        /// <summary>
        /// Create a new Project
        /// </summary>
        public NpcChatProject()
        {
            ProjectCharacters = new CharacterStore();
            ProjectDialogs = new DialogManager(this);
            ProjectStoryElements = new StoryElementManager();
        }

        /// <summary>
        /// Load a project from an existing source
        /// </summary>
        /// <param name="path">path to project source</param>
        public NpcChatProject(string path) : this()
        {
            ProjectCharacters.RegisterNewCharacter(out int _, new Character("Fiona"));
        }

        // convenient lookups
        public DialogTree this[DialogTreeIdentifier id] => ProjectDialogs[id];
        public DialogTreeBranch this[DialogTreeBranchIdentifier id] => ProjectDialogs[id];
        public DialogSegment this[DialogSegmentIdentifier id] => ProjectDialogs[id];
        public Character? this[CharacterId id] => ProjectCharacters[id];
    }
}