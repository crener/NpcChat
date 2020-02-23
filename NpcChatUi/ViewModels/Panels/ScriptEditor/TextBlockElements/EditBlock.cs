using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Schema;
using NLog;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Panels.ScriptEditor.TextBlockElements
{
    public class EditBlock : Run
    {
        private readonly Action<object, PropertyChangedEventArgs> m_changedEvent;
        private readonly IDialogElement m_element;
        private readonly bool m_initialized = false;
        protected string m_text;

        public EditBlock(string text, IDialogElement element, Action<object, PropertyChangedEventArgs> changedEvent = null)
            : base(text)
        {
            m_initialized = true;
            m_text = text;
            m_element = element;
            m_changedEvent = changedEvent;

            m_element.PropertyChanged += OnElementPropertyChanged;
        }


        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!m_initialized) return;
            if (e.Property.Name == nameof(Text) && Text != m_text)
            {
                if (m_element.Edit(m_text, Text))
                {
                    m_text = Text;
                }
            }
        }

        private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IDialogElement.Text))
            {
                string elementText = m_element.Text;
                if (Text != elementText)
                {
                    m_text = elementText;
                    Text = m_text;

                    m_changedEvent?.Invoke(this, null);
                }
            }
        }
    }
}
