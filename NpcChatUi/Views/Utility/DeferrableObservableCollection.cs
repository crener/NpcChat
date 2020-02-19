using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Properties;

namespace NpcChat.Views.Utility
{
    /// <summary>
    /// Observable collection which is able to temporarily stop PropertyChanged events
    /// </summary>
    public class DeferrableObservableCollection<T> : ObservableCollection<T>
    {
        protected override event PropertyChangedEventHandler PropertyChanged
        {
            add => InternalPropertyChanged += value;
            remove => InternalPropertyChanged -= value;
        }

        public bool DeferringChanges { get; private set; }
        public bool ChangesOccuredDuringLastDefer { get; private set; }

        private event PropertyChangedEventHandler InternalPropertyChanged;

        public DeferrableObservableCollection()
        {
            base.PropertyChanged += BaseCollectionChanged;
        }

        public DeferrableObservableCollection([NotNull] List<T> list) : base(list)
        {
            base.PropertyChanged += BaseCollectionChanged;
        }

        public DeferrableObservableCollection([NotNull] IEnumerable<T> collection) : base(collection)
        {
            base.PropertyChanged += BaseCollectionChanged;
        }

        public DeferScope CreateDeferringScope()
        {
            return new DeferScope(this);
        }

        /// <summary>
        /// Start to ignore changes until <see cref="StopDeferring"/> is called
        /// </summary>
        public void StartToDefer()
        {
            DeferringChanges = true;
            ChangesOccuredDuringLastDefer = false;
        }

        /// <summary>
        /// Start passing property change events through again and call <see cref="PropertyChanged"/> if a change was made
        /// </summary>
        public void StopDeferring()
        {
            DeferringChanges = false;

            if(ChangesOccuredDuringLastDefer)
            {
                InternalPropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
                InternalPropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            }
        }

        private void BaseCollectionChanged(object sender, PropertyChangedEventArgs args)
        {
            if(!DeferringChanges)
            {
                InternalPropertyChanged?.Invoke(this, args);
            }
            else
            {
                ChangesOccuredDuringLastDefer = true;
            }
        }

        public class DeferScope : IDisposable
        {
            private readonly DeferrableObservableCollection<T> m_collection;

            internal DeferScope(DeferrableObservableCollection<T> collection)
            {
                m_collection = collection;
                collection.StartToDefer();
            }

            public void Dispose()
            {
                m_collection.StopDeferring();
            }
        }
    }
}
