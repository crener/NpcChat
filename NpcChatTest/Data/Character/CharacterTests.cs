using System.CodeDom.Compiler;
using System.Linq;
using NpcChatSystem;
using NUnit.Framework;
using NpcChatSystem.Data.CharacterData;

namespace NpcChatTest.Data.Character
{
    [TestFixture]
    public class CharacterTests
    {
        [TestCase("")]
        [TestCase(null)]
        [TestCase("Duke")]
        public void NewCharIdTestPlain(string name)
        {
            NpcChatProject project = new NpcChatProject();
            bool result = project.ProjectCharacters.RegisterNewCharacter(out int gen, name);

            Assert.IsTrue(result);
            Assert.AreNotEqual(NpcChatSystem.Data.CharacterData.Character.PreRegisteredId, gen);
            Assert.AreEqual(project.ProjectCharacters.GetCharacter(gen).Id, gen);
        }

        
    }
}
