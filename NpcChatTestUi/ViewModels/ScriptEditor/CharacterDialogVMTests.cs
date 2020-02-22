using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChat.Backend.Interfaces;
using NpcChat.ViewModels.Panels.ScriptEditor;
using NpcChat.Views.Utility;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogParts;
using NUnit.Framework;

namespace NpcChatTestUi.ViewModels.ScriptEditor
{
    public class CharacterDialogVMTests
    {
        [Test]
        public void ElementTextEditTestInspectThenEdit()
        {
            CharacterDialogVM testVM = ElementTextEditTestCreation();
            DialogSegment dialogSegment = testVM.DialogSegment;

            testVM.InspectionActive = true;
            testVM.EditMode = EditMode.TextBlock;
            Assert.AreEqual("", testVM.DialogDocument.Text());
            testVM.EditMode = EditMode.Elements;

            // add simple text element
            string elementName = new DialogText().ElementName;
            Assert.IsTrue(testVM.AddDialogElementCommand.CanExecute(elementName));
            testVM.AddDialogElementCommand.Execute(elementName);
            Assert.AreEqual(1, testVM.DialogSegment.SegmentParts.Count);

            // edit simple text
            const string simpleText = "This is some simple text";
            DialogText element = testVM.DialogSegment.SegmentParts.First() as DialogText;
            Assert.NotNull(element);
            element.Text = simpleText;

            // check that all of the characterDialogVMs text matches
            Assert.AreEqual(simpleText, element.Text);
            Assert.AreEqual(simpleText, dialogSegment.Text);
            testVM.EditMode = EditMode.TextBlock;
            Assert.AreEqual(simpleText, testVM.DialogDocument.Text());
        }

        [Test]
        public void ElementTextEditTestEditThenInspect()
        {
            CharacterDialogVM testVM = ElementTextEditTestCreation();
            DialogSegment dialogSegment = testVM.DialogSegment;

            testVM.EditMode = EditMode.TextBlock;
            testVM.InspectionActive = true;
            Assert.AreEqual("", testVM.DialogDocument.Text());
            testVM.EditMode = EditMode.Elements;

            // add simple text element
            string elementName = new DialogText().ElementName;
            Assert.IsTrue(testVM.AddDialogElementCommand.CanExecute(elementName));
            testVM.AddDialogElementCommand.Execute(elementName);
            Assert.AreEqual(1, dialogSegment.SegmentParts.Count);

            // edit simple text
            const string simpleText = "This is some simple text";
            DialogText element = dialogSegment.SegmentParts.First() as DialogText;
            Assert.NotNull(element);
            element.Text = simpleText;

            // check that all of the characterDialogVMs text matches
            Assert.AreEqual(simpleText, element.Text);
            Assert.AreEqual(simpleText, dialogSegment.Text);
            testVM.EditMode = EditMode.TextBlock;
            Assert.AreEqual(simpleText, testVM.DialogDocument.Text());
        }

        private CharacterDialogVM ElementTextEditTestCreation()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            Assert.AreEqual(0, branch.Dialog.Count, "This is a new branch and shouldn't have any dialog inside it!");

            int charId;
            Assert.IsTrue(project.ProjectCharacters.RegisterNewCharacter(out charId, "Gerald"));
            DialogSegment dialogSegment = branch.CreateNewDialog(charId);
            Assert.AreEqual(1, branch.Dialog.Count, "Failed to create sample dialog segment");

            dialogSegment.ClearElements();
            Assert.AreEqual(0, dialogSegment.SegmentParts.Count);
            Assert.AreEqual("", dialogSegment.Text);

            CharacterDialogVM testVM = new CharacterDialogVM(project, dialogSegment)
            {
                EditMode = EditMode.Elements,
                InspectionActive = false
            };

            // make sure creation was valid
            Assert.NotNull(testVM);
            Assert.AreSame(dialogSegment, testVM.DialogSegment);
            Assert.AreEqual(0, testVM.DialogSegment.SegmentParts.Count);

            return testVM;
        }
    }
}
