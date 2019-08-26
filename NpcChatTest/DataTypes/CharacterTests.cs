using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.CharacterData;
using NUnit.Framework;

namespace NpcChatTest.DataTypes
{
    [TestFixture]
    public class CharacterTests
    {
        #region Default id tests
        // The character Id should always be 0 when first making a character as otherwise registration checks will fail to make a correct id

        [Test]
        public void NewCharIdTestPlain()
        {
            Character character = new Character();
            Assert.AreEqual(Character.PreRegisteredId, character.Id);
        }

        [Test]
        public void NewCharIdTestName()
        {
            Character character = new Character("Daisy");
            Assert.AreEqual(Character.PreRegisteredId, character.Id);
        }
        #endregion
    }
}
