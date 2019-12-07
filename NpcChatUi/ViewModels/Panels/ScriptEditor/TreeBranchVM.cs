using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NpcChat.Backend.Interfaces;
using NpcChat.Properties;
using NpcChat.Util;
using NpcChat.ViewModels.Panels.ScriptEditor.Util;
using NpcChatSystem;
using NpcChatSystem.Branching.EvaluationContainers;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;
using NpcChatSystem.System.TypeStore.Stores;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Panels.ScriptEditor
{
    [DebuggerDisplay("BranchVM: {DialogBranch.Name}")]
    public class TreeBranchVM : NotificationObject
    {
        // general properties which shouldn't really change over time
        public NpcChatProject Project { get; }
        public IReadOnlyCollection<string> EvaluationContainers => EvaluationContainerTypeStore.ContainerNames;

        public ObservableCollection<CharacterDialogVM> Speech { get; } = new ObservableCollection<CharacterDialogVM>();
        public DialogTreeBranch DialogBranch { get; }

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

        public EditMode EditMode
        {
            get { return m_editMode; }
            set
            {
                if (m_editMode == value) return;
                
                m_editMode = value;

                foreach(CharacterDialogVM vm in Speech)
                    vm.EditMode = value;
                RaisePropertyChanged();
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
                    IEvaluationContainer container = DialogBranch?.BranchCondition;
                    if (container == null && DialogBranch != null)
                    {
                        m_evaluationCacheName = SimpleEvaluationContainer.Name;
                        DialogBranch.BranchCondition = EvaluationContainerTypeStore.Instance.CreateEntity(m_evaluationCacheName);
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
                if (DialogBranch == null)
                {
                    Logging.Logger.Warn($"Unable to change EvaluationContainer due to null '{nameof(DialogTreeBranch)}'");
                    return;
                }

                m_evaluationCacheName = value;
                IEvaluationContainer newContainer = EvaluationContainerTypeStore.Instance.CreateEntity(m_evaluationCacheName);
                DialogBranch.BranchCondition = newContainer;

                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The branches which are linked to this branch
        /// </summary>
        public ObservableCollection<TreeBranchLinkInfoVM> BranchLinks { get; } = new ObservableCollection<TreeBranchLinkInfoVM>();
        public int VisibleBranchLinkIndex
        {
            get => m_visibleBranchLinkIndex;
            set
            {
                if (m_visibleBranchLinkIndex == value) return;

                m_visibleBranchLinkIndex = value;
                if (m_visibleBranchLinkIndex < 0) return;

                BranchLinks[m_visibleBranchLinkIndex].RebaseScriptView.Execute(m_script.Branches);
                ScriptVisibleBranchesChanged(m_script.Branches);
                RaisePropertyChanged(nameof(VisibleBranchLinkIndex));
            }
        }

        /// <summary>
        /// Branches which could be linked to the branch
        /// </summary>
        public ObservableCollection<DialogTreeBranch> PotentialBranchLinks { get; } = new ObservableCollection<DialogTreeBranch>();
        public bool AreBranchLinksPossible => PotentialBranchLinks.Any();
        public int SelectedBranchLinkIndex
        {
            get => m_selectedBranchLinkIndex;
            set
            {
                m_selectedBranchLinkIndex = value;
                RaisePropertyChanged();
            }
        }

        // commands
        public ICommand AddNewDialogCommand { get; }
        public ICommand InsertBranchCommand { get; }
        public ICommand LinkBranchCommand { get; }
        public ICommand DeleteBranchCommand { get; }

        private DialogTree m_tree => Project[(DialogTreeIdentifier)DialogBranch];
        private IScriptPanelVM m_script;
        private int? m_newDialogCharacterId = null;
        private string m_evaluationCacheName = null;
        private int m_visibleBranchLinkIndex = -1;
        private int m_selectedBranchLinkIndex;
        private EditMode m_editMode;

        public TreeBranchVM(NpcChatProject project, IScriptPanelVM script, [NotNull] DialogTreeBranchIdentifier dialogTreeId)
            : this(project, script, project[dialogTreeId]) { }

        public TreeBranchVM(NpcChatProject project, IScriptPanelVM script, [NotNull] DialogTreeBranch dialogBranch)
        {
            Project = project;
            DialogBranch = dialogBranch;
            m_script = script;

            if (m_tree != null)
            {
                m_tree.OnBranchCreated += branch =>
                {
                    if (!m_tree.CheckForCircularDependency(DialogBranch.Id, branch))
                        PotentialBranchLinks.Add(branch);
                };
                m_tree.OnBranchRemoved += branch =>
                {
                    if (PotentialBranchLinks.Contains(branch))
                    {
                        CheckBranchCompatibility();
                    }
                };
            }

            if (dialogBranch != null)
            {
                //add existing branching options
                foreach (DialogTreeBranchIdentifier child in DialogBranch.Children)
                {
                    BranchLinks.Add(CreateTreeBranchLink(child));
                }

                // register callbacks
                DialogBranch.OnDialogCreated += added =>
                {
                    if (!Project.ProjectDialogs.HasDialog(added)) return;
                    Speech.Add(new CharacterDialogVM(Project, Project.ProjectDialogs[added]));
                };
                DialogBranch.OnDialogDestroyed += removed =>
                {
                    Speech.Clear();
                    foreach (DialogSegment segment in dialogBranch.Dialog)
                        Speech.Add(new CharacterDialogVM(Project, segment));
                };
                DialogBranch.OnBranchChildAdded += (id) =>
                {
                    BranchLinksChanged(id, true);
                    CheckPotentialBranchLink(id);
                };
                DialogBranch.OnBranchChildRemoved += (id) =>
                {
                    BranchLinksChanged(id, false);
                    CheckPotentialBranchLink(id);
                };
                DialogBranch.OnBranchParentAdded += id => { CheckBranchCompatibility(); };
                DialogBranch.OnBranchParentRemoved += id => { CheckBranchCompatibility(); };
            }

            script.OnVisibleBranchChange += ScriptVisibleBranchesChanged;

            {
                //Possible pre existing branch links
                CheckBranchCompatibility();

                PotentialBranchLinks.CollectionChanged += (sender, args) =>
                {
                    if (PotentialBranchLinks.Count == 1) SelectedBranchLinkIndex = 0;
                    else if (PotentialBranchLinks.Count == 0) SelectedBranchLinkIndex = -1;
                    RaisePropertyChanged(nameof(AreBranchLinksPossible));
                };

                LinkBranchCommand = new DelegateCommand<DialogTreeBranch>((link) =>
                {
                    DialogBranch.AddChild(link);
                    PotentialBranchLinks.Remove(link);
                });
                InsertBranchCommand = new DelegateCommand(() =>
                {
                    m_script.AddNewBranch(DialogBranch.Id, true);
                });
            }

            AddNewDialogCommand = new DelegateCommand(() =>
            {
                DialogBranch?.CreateNewDialog(NewDialogCharacterId);
            }, () => NewDialogCharacterId != CharacterId.DefaultId);
            DeleteBranchCommand = new DelegateCommand(() =>
            {
                m_tree.RemoveBranch(DialogBranch.Id);
            });
        }

        /// <summary>
        /// Update the selected branch index 
        /// </summary>
        /// <param name="branches">collection of active tree branches</param>
        private void ScriptVisibleBranchesChanged(IReadOnlyList<TreeBranchVM> branches)
        {
            if (!branches.Contains(this)) return;

            int activeIndex = -1;
            for (int i = 0; i < branches.Count; i++)
            {
                TreeBranchVM branchVM = branches[i];
                if (branchVM == this)
                {
                    if (i + 1 < branches.Count)
                    {
                        DialogTreeBranchIdentifier treeId = branches[i + 1].DialogBranch.Id;
                        for (int l = 0; l < BranchLinks.Count; l++)
                        {
                            TreeBranchLinkInfoVM treeBranchLinkInfoVM = BranchLinks[l];
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

            VisibleBranchLinkIndex = activeIndex;
        }

        private void BranchLinksChanged(DialogTreeBranchIdentifier id, bool added)
        {
            if (added)
            {
                BranchLinks.Add(CreateTreeBranchLink(id));
            }
            else
            {
                for (int i = 0; i < BranchLinks.Count; i++)
                {
                    TreeBranchLinkInfoVM linkInfo = BranchLinks[i];
                    if (id == linkInfo.Child) BranchLinks.Remove(linkInfo);
                }
            }
        }

        private void CheckBranchCompatibility()
        {
            PotentialBranchLinks.Clear();
            foreach (DialogTreeBranchIdentifier dialog in m_tree.Branches)
            {
                CheckPotentialBranchLink(dialog);
            }
        }

        private void CheckPotentialBranchLink(DialogTreeBranchIdentifier id)
        {
            DialogTree tree = Project[DialogBranch.Id as DialogTreeIdentifier];
            bool circle = tree.CheckForCircularDependency(DialogBranch.Id, id);

            if (circle || BranchLinks.Any(s => s.Child == id)) PotentialBranchLinks.Remove(tree[id]);
            else PotentialBranchLinks.Add(tree[id]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TreeBranchLinkInfoVM CreateTreeBranchLink(DialogTreeBranchIdentifier id)
        {
            return new TreeBranchLinkInfoVM(Project, m_script, DialogBranch.Id, id);
        }
    }
}
