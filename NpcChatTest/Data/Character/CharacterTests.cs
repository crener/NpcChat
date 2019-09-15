using NUnit.Framework;

namespace NpcChatTest.Data.Character
{
    [TestFixture]
    public class CharacterTests
    {
        #region Default id tests
        // The character Id should always be 0 when first making a character as otherwise registration checks will fail to make a correct id

        [Test]
        public void NewCharIdTestPlain()
        {
            NpcChatSystem.Data.CharacterData.Character character = new NpcChatSystem.Data.CharacterData.Character();
            Assert.AreEqual(NpcChatSystem.Data.CharacterData.Character.PreRegisteredId, character.Id);
        }

        [Test]
        public void NewCharIdTestName()
        {
            NpcChatSystem.Data.CharacterData.Character character = new NpcChatSystem.Data.CharacterData.Character("Daisy");
            Assert.AreEqual(NpcChatSystem.Data.CharacterData.Character.PreRegisteredId, character.Id);
        }
        #endregion
    }
}
