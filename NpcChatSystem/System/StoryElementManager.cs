using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data;

namespace NpcChatSystem.System
{
    public class StoryElementManager
    {
        List<StoryElement> m_elements = new List<StoryElement>();

        /// <summary>
        /// Find a story element 
        /// </summary>
        /// <param name="name">name of the storyElement</param>
        /// <returns>character if found, null if not found</returns>
        public StoryElement GetElement(string name)
        {
            return m_elements.FirstOrDefault(c => c.Name == name);
        }
    }
}
