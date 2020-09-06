using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DynamicData;
using NodeNetwork.ViewModels;
using NpcChat.ViewModels.Panels.ScriptDiagram.Layout;
using NpcChat.ViewModels.Panels.ScriptDiagram.Node;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTestUi.ViewModels.ScriptDiagram.Layout
{
    public class LeveledLayoutTests : BasicLayoutTests
    {
        protected override ILayout CreateLayoutInstance()
        {
            return new MsAglLayout();
        }
    }

    public abstract class BasicLayoutTests
    {
        protected abstract ILayout CreateLayoutInstance();

        [Test]
        public void NoNetwork()
        {
            ILayout layout = CreateLayoutInstance();
            layout.Layout(null);
            
            layout.Layout(null);
        }

        [Test]
        public void NoNodes()
        {
            NetworkViewModel network = new NetworkViewModel();
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);
            
            Assert.AreEqual(0, network.Nodes.Count);
        }
    }
}