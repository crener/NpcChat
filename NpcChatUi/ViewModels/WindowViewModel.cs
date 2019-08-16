using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.DialogTreeItems;

namespace NpcChat.ViewModels
{
    class WindowViewModel : NotificationObject
    {
        public int TreeId { get; set; }

        public WindowViewModel()
        {
            NpcChatProject project = new NpcChatProject();
            if (project.ProjectCharacters.RegisterNewCharacter(out int diane, new Character("diane")) &&
                project.ProjectCharacters.RegisterNewCharacter(out int jerry, new Character("jerry")))
            {
                DialogTree dialog = project.ProjectDialogs.CreateNewDialogTree();
                TreePart branch = dialog.CreateNewBranch();
                DialogSegment segment = branch.CreateNewDialog(diane);
                DialogSegment segment2 = branch.CreateNewDialog(jerry);

                TreeId = branch.DialogTreeId;
            }
        }
    }
}
