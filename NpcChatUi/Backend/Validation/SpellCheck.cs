using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;
using System.Windows.Documents;
using NpcChat.ViewModels.Panels.ScriptEditor.TextBlockElements;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Utilities;
using WeCantSpell.Hunspell;
using LogLevel = NLog.LogLevel;

namespace NpcChat.Backend.Validation
{
    public class SpellCheck
    {
        private static Dictionary<string, WordList> m_dictionary;
        private static readonly string[] s_acceptedFormats = new[] { ".dic", ".aff" };

        static SpellCheck()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            string dictionaryDir = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SpellCheck");
            m_dictionary = new Dictionary<string, WordList>();

            // parse file paths to get
            Dictionary<string, string[]> filePair = new Dictionary<string, string[]>();
            foreach (string path in Directory.EnumerateFiles(dictionaryDir))
            {
                string type = Path.GetFileNameWithoutExtension(path)?.ToLower();
                string end = Path.GetExtension(path)?.ToLower();

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(end)) continue;
                if (!s_acceptedFormats.Contains(end)) continue;
                if (!filePair.ContainsKey(type)) filePair[type] = new string[2];

                if (end == ".dic") filePair[type][0] = path;
                if (end == ".aff") filePair[type][1] = path;
            }

            Logging.Logger.Log(LogLevel.Info, $"Found {filePair.Count} dictionaries, starting import");

            foreach (KeyValuePair<string, string[]> pair in filePair)
            {
                if (pair.Value[0] == null) continue;

                try
                {
                    WordList dictionary = WordList.CreateFromFiles(pair.Value[0], pair.Value[1]);
                    m_dictionary.Add(pair.Key, dictionary);

                    Logging.Logger.Log(LogLevel.Info, $"Imported '{pair.Key}' language dictionary");
                }
                catch (Exception ex)
                {
                    Logging.Logger.Log(LogLevel.Error, ex, $"Unable to initialize spell check directory ('{pair.Key}')");
                }
            }

            timer.Stop();
            Logging.Logger.Log(LogLevel.Info, $"{m_dictionary.Count} Spelling dictionaries imported, took {timer.Elapsed.TotalSeconds} seconds");
        }

        /// <summary>
        /// Perform spell check on the text from the element
        /// </summary>
        /// <param name="element">element to check</param>
        /// <param name="currentLanguage">language code to use spell check for</param>
        public static IEnumerable<Inline> CheckElement(IDialogElement element, string currentLanguage = "en_gb")
        {
            List<Inline> paragraphRuns = new List<Inline>();

            if (m_dictionary.ContainsKey(currentLanguage))
            {
                WordList dictionary = m_dictionary[currentLanguage];

                string fullText = element.Text;
                string[] words = fullText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string sentence = fullText.StartsWith(" ") ? " " : "";

                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];
                    List<string> suggestions = dictionary.Suggest(word).ToList();
                    if (suggestions.Any() && !suggestions.Contains(word))
                    {
                        if (i > 0) sentence += " ";
                        paragraphRuns.Add(new Run(sentence));
                        sentence = "";

                        paragraphRuns.Add(new Bold(new SpellingOptions(word, element, suggestions.Except(new[] { word }))));
                    }
                    else
                    {
                        if (i > 0) sentence += " ";
                        sentence += word;
                    }
                }

                //add any remaining words
                if (fullText.EndsWith(" ")) sentence += " ";
                if (sentence.Length > 0)
                {
                    paragraphRuns.Add(new Run(sentence));
                }
            }
            else
            {
                paragraphRuns.Add(new Run(element.Text));
            }

            return paragraphRuns;
        }

        /// <summary>
        /// Check the given dialog element for spelling errors
        /// </summary>
        /// <param name="elements">text to check</param>
        /// <param name="currentLanguage">language code too check against</param>
        /// <returns>true if sentence contains an error</returns>
        public static bool ContainsSpellingSuggestion(IEnumerable<IDialogElement> elements, string currentLanguage = "en_gb")
        {
            return elements.Any(e => ContainsSpellingSuggestion(e.Text, currentLanguage));
        }

        /// <summary>
        /// Check the given dialog element for spelling errors
        /// </summary>
        /// <param name="element">text to check</param>
        /// <param name="currentLanguage">language code too check against</param>
        /// <returns>true if sentence contains an error</returns>
        public static bool ContainsSpellingSuggestion(IDialogElement element, string currentLanguage = "en_gb")
        {
            return ContainsSpellingSuggestion(element.Text, currentLanguage);
        }

        /// <summary>
        /// Check the given string for spelling errors
        /// </summary>
        /// <param name="sentence">text to check</param>
        /// <param name="currentLanguage">language code too check against</param>
        /// <returns>true if sentence contains an error</returns>
        public static bool ContainsSpellingSuggestion(string sentence, string currentLanguage = "en_gb")
        {
            if (m_dictionary.ContainsKey(currentLanguage))
            {
                foreach (string word in sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    List<string> suggestions = m_dictionary[currentLanguage].Suggest(word).ToList();
                    if (suggestions.Any() && !suggestions.Contains(word))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
