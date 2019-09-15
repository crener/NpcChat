using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.System.TypeStore;
using NUnit.Framework;

namespace NpcChatTest.Data.Dialog.DialogParts
{
    class DialogTextTests
    {
        [Test]
        public void Instantiate()
        {
            DialogText element = DialogTypeStore.CreateDialogElement<DialogText>();
            Assert.NotNull(element);
            Assert.NotNull(element.Text);
        }

        [Test]
        public void HasElementName()
        {
            DialogText element = DialogTypeStore.CreateDialogElement<DialogText>();
            Assert.NotNull(element.ElementName);
        }

        [Test]
        public void TextChangedCallback()
        {
            DialogText element = DialogTypeStore.CreateDialogElement<DialogText>();

            bool changed = false;
            element.PropertyChanged += (s, a) => { changed = true; };

            const string newText = "This is a text";
            element.Text = newText;
            Assert.IsTrue(changed, "Failed to trigger PropertyChanged callback");
            Assert.AreEqual(newText, element.Text);
        }
    }
}
