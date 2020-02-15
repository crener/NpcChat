using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using NLog;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Panels.ScriptEditor.TextBlockElements
{
    public class SpellingOptions : Run
    {
        private static Pen s_underline = new Pen(Brushes.Red, 2);

        private readonly ContextMenu m_menu;
        private readonly string m_text;
        private readonly IDialogElement m_element;

        public SpellingOptions(string text, IDialogElement element, string[] corrections = null) : base(text)
        {
            m_text = text;
            m_element = element;

            TextDecoration decoration = new TextDecoration(TextDecorationLocation.Underline, s_underline, 0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended);
            TextDecorations.Add(decoration);

            m_menu = new ContextMenu();
            m_menu.Placement = PlacementMode.MousePoint;

            if (corrections != null)
            {
                DelegateCommand<string> replaceTextCmd = new DelegateCommand<string>(ReplaceText);
                foreach (string correction in corrections)
                {
                    m_menu.Items.Add(new MenuItem
                    {
                        Header = correction,
                        Command = replaceTextCmd,
                        CommandParameter = correction,
                    });
                }

                m_menu.Items.Add(new Separator());
            }

            m_menu.Items.Add(new MenuItem
            {
                Header = $"Add \"{text}\" to dictionary",
                Command = new DelegateCommand(AddToDictionary)
            });
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right) return;
            e.Handled = true;

            m_menu.IsOpen = true;
        }

        private void ReplaceText(string replacement)
        {
            Logging.Logger.Log(LogLevel.Info, $"SpellCheck: Replacing '{m_text}' with '{replacement}'");

            if (!m_element.IntegrateCorrection(m_text, replacement))
            {
                Logging.Logger.Log(LogLevel.Info, $"SpellCheck: Replacement operation ('{m_text}' -> '{replacement}') failed");
            }
        }

        private void AddToDictionary()
        {
            throw new NotImplementedException();
        }
    }
}
