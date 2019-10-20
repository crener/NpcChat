using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NpcChat.Properties;
using NpcChat.Util;
using NpcChat.ViewModels.Editors.Script.Util;
using NpcChatSystem;
using NpcChatSystem.Branching.EvaluationContainers;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.System.TypeStore.Stores;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Editors.Script
{
    [DebuggerDisplay("BranchVM: {DialogTree.Name}")]
    public class TreeBranchVM : NotificationObject
    {
        // general properties which shouldn't really change over time
        public NpcChatProject Project { get; }
        public IReadOnlyCollection<string> EvaluationContainers => EvaluationContainerTypeStore.ContainerNames;

        public ObservableCollection<CharacterDialogVM> Speech { get; } = new ObservableCollection<CharacterDialogVM>();
        public DialogTreeBranch DialogTree { get; }

        public int NewDialogCharacterId
        {
            get
            {
                if (!m_newDialogCharacterId.HasValue)
                {
                    if (Speech.Count == 0)
                    {
                        IList<CharacterId> characters = Project.ProjectCharacters.AvailableCharacters();
                        if (characters.Count == 0) return CharacterId.DefaultId;
                        return characters[0].Id;
                    }
                    return Speech.Last().CharacterId;
                }

                return m_newDialogCharacterId.Value;
            }
            set
            {
                m_newDialogCharacterId = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(AddNewDialogCommand));
            }
        }

        /// <summary>
        /// Name of the used evaluation container
        /// </summary>
        public string UsedEvaluationContainer
        {
            get
            {
                if (m_evaluationCacheName == null)
                {
                    IEvaluationContainer container = DialogTree?.BranchCondition;
                    if (container == null && DialogTree != null)
                    {
                        m_evaluationCacheName = SimpleEvaluationContainer.Name;
                        DialogTree.BranchCondition = EvaluationContainerTypeStore.Instance.CreateEntity(m_evaluationCacheName);
                    }
                    else
                    {
                        m_evaluationCacheName = EvaluationContainerTypeStore.Instance.GetContainerKey(container);
                    }
                }

                return m_evaluationCacheName;
            }
            set
            {
                if (m_evaluationCacheName == value) return;
                if (DialogTree == null)
                {
                    Logging.Logger.Error($"Unable to change EvaluationContainer due to null '{nameof(DialogTreeBranch)}'");
                    return;
                }

                m_evaluationCacheName = value;
                IEvaluationContainer newContainer = EvaluationContainerTypeStore.Instance.CreateEntity(m_evaluationCacheName);
                DialogTree.BranchCondition = newContainer;

                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TreeBranchLinkInfoVM> BranchOptions { get; } = new ObservableCollection<TreeBranchLinkInfoVM>();
        public int VisibleBranchIndex
        {
            get => m_visibleBranchIndex;
            set
            {
                if(m_visibleBranchIndex == value) return;

                m_visibleBranchIndex = value;

                BranchOptions[m_visibleBranchIndex].RebaseScriptView.Execute(m_script.Branches);
                VisibleBranchesChanged(m_script.Branches);
                RaisePropertyChanged(nameof(VisibleBranchIndex));
            }
        }

        // commands
        public ICommand AddNewDialogCommand { get; }
        public ICommand CreateNewBranchChildCommand { get; }

        private ScriptPanelVM m_script;
        private int? m_newDialogCharacterId = null;
        private string m_evaluationCacheName = null;
        private int m_visibleBranchIndex = -1;

        public TreeBranchVM(NpcChatProject project, ScriptPanelVM script, [NotNull] DialogTreeBranchIdentifier dialogTreeId)
            : this(project, script, project[dialogTreeId]) { }

        public TreeBranchVM(NpcChatProject project, ScriptPanelVM script, [NotNull] DialogTreeBranch dialogTree)
        {
            Project = project;
            DialogTree = dialogTree;
            m_script = script;

            if (dialogTree != null)
            {
                dialogTree.OnDialogCreated += added =>
                {
                    if (!Project.ProjectDialogs.HasDialog(added)) return;
                    Speech.Add(new CharacterDialogVM(Project, Project.ProjectDialogs[added]));
                };
                dialogTree.OnDialogDestroyed += removed =>
                {
                    Speech.Clear();
                    foreach (DialogSegment segment in dialogTree.Dialog)
                        Speech.Add(new CharacterDialogVM(Project, segment));
                };

                //add existing branching options
                foreach (DialogTreeBranchIdentifier child in DialogTree.Children)
                {
                    BranchOptions.Add(CreateTreeBranchLink(child));
                }
            }

            DialogTree.OnBranchChildAdded += (id) => BranchLinksChanged(id, true);
            DialogTree.OnBranchChildRemoved += (id) => BranchLinksChanged(id, false);

            AddNewDialogCommand = new DelegateCommand(() =>
            {
                DialogTree.CreateNewDialog(NewDialogCharacterId);
            }, () => NewDialogCharacterId != CharacterId.DefaultId);

            CreateNewBranchChildCommand = new DelegateCommand(() =>
            {
                m_script.AddNewBranch(DialogTree.Id, true);
            });

            script.OnVisibleBranchChange += VisibleBranchesChanged;
        }

        /// <summary>
        /// Update the selected branch index 
        /// </summary>
        /// <param name="branches">collection of active tree branches</param>
        private void VisibleBranchesChanged(IReadOnlyList<TreeBranchVM> branches)
        {
            if(!branches.Contains(this)) return;

            int activeIndex = -1;
            for (int i = 0; i < branches.Count; i++)
            {
                TreeBranchVM branchVM = branches[i];
                if (branchVM == this)
                {
                    if (i + 1 < branches.Count)
                    {
                        DialogTreeBranchIdentifier treeId = branches[i + 1].DialogTree.Id;
                        for (int l = 0; l < BranchOptions.Count; l++)
                        {
                            TreeBranchLinkInfoVM treeBranchLinkInfoVM = BranchOptions[l];
                            if (treeBranchLinkInfoVM.Child == treeId)
                            {
                                activeIndex = l;
                                break;
                            }
                        }
                    }
                    break;
                }
            }

            VisibleBranchIndex = activeIndex;
        }

        private void BranchLinksChanged(DialogTreeBranchIdentifier id, bool added)
        {
            if (added)
            {
                BranchOptions.Add(CreateTreeBranchLink(id));
            }
            else
            {
                for (int i = 0; i < BranchOptions.Count; i++)
                {
                    TreeBranchLinkInfoVM linkInfo = BranchOptions[i];
                    if (id == linkInfo.Child) BranchOptions.Remove(linkInfo);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TreeBranchLinkInfoVM CreateTreeBranchLink(DialogTreeBranchIdentifier id)
        {
            return new TreeBranchLinkInfoVM(Project, m_script, DialogTree.Id, id);
        }
    }
}
