using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NpcChatSystem;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.System;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Panels.Project
{
    public class CharacterOverview : INotifyPropertyChanged
    {
        public string Name => m_character.Name;
        public int Id => m_character.Id;

        public uint DialogCount
        {
            get => m_dialogCount;
            private set
            {
                m_dialogCount = value;
                OnPropertyChanged();
            }
        }
        public uint TreeCount
        {
            get => m_treeCount;
            private set
            {
                m_treeCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateStats { get; }
        public ICommand ShowEditorCommand { get; }

        private NpcChatProject m_project;
        private Character m_character;
        private readonly List<DialogTreeBranchIdentifier> m_referencedBranchs = new List<DialogTreeBranchIdentifier>();
        private readonly List<DialogTreeIdentifier> m_referencedTrees = new List<DialogTreeIdentifier>();
        private uint m_dialogCount, m_treeCount;

        private CharacterOverview(NpcChatProject project, Character character)
        {
            m_project = project;
            m_character = character;

            ShowEditorCommand = new DelegateCommand(EditCharacter);
            UpdateStats = new DelegateCommand(UpdateUsageStatistics);
            UpdateUsageStatistics();
        }

        private void EditCharacter()
        {
            //todo link when there is a character editor
            throw new NotImplementedException();
        }

        private void UpdateUsageStatistics()
        {
            m_referencedBranchs.Clear();
            m_referencedTrees.Clear();
            m_dialogCount = m_treeCount = 0;

            foreach (DialogTreeIdentifier treeId in m_project.ProjectDialogs.DialogTreeIds)
            {
                bool treeReferenced = false;
                DialogTree tree = m_project[treeId];
                foreach (DialogTreeBranchIdentifier branchId in tree.Branches)
                {
                    bool branchReferenced = false;
                    DialogTreeBranch branch = m_project[branchId];
                    foreach (DialogSegment dialog in branch.Dialog)
                    {
                        if (dialog.CharacterId == Id)
                        {
                            DialogCount++;
                            branchReferenced = true;
                            treeReferenced = true;
                        }
                    }

                    if (branchReferenced)
                    {
                        m_referencedBranchs.Add(branchId);
                    }
                }

                if (treeReferenced)
                {
                    TreeCount++;
                    m_referencedTrees.Add(treeId);
                }
            }
        }

        public static IEnumerable<CharacterOverview> AnalyseProject(NpcChatProject project)
        {
            Logging.Logger.Info("Gathering Chararacter statistics from current project");
            List<CharacterOverview> lookup = new List<CharacterOverview>();

            foreach (CharacterId id in project.ProjectCharacters.AvailableCharacters())
            {
                Character character = project[id];
                if (character == null)
                {
                    Logging.Logger.Error($"Unable to find expected character '{id}' but this retrieved from {nameof(CharacterStore)}.{nameof(CharacterStore.AvailableCharacters)} so should be availible");
                    continue;
                }

                CharacterOverview overview = new CharacterOverview(project, character);
                lookup.Add(overview);
            }

            Logging.Logger.Info("Chararacter project statistics done!");
            return lookup;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
