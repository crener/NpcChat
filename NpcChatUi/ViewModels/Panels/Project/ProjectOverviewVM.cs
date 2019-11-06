using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using Xceed.Wpf.AvalonDock.Layout;

namespace NpcChat.ViewModels.Panels.Project
{
    public class ProjectOverviewVM : LayoutDocument
    {
        private NpcChatProject m_project { get; }

        public ProjectOverviewVM(NpcChatProject project)
        {
            m_project = project;
            Title = "Project Overview";
        }


    }
}
