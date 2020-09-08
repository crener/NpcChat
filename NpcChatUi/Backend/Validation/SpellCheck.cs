using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using NpcChat.ViewModels.Panels.ScriptEditor.TextBlockElements;
using NpcChat.ViewModels.Settings;
using NpcChat.ViewModels.Settings.SettingsTabs;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Utilities;
using WeCantSpell.Hunspell;
using LogLevel = NLog.LogLevel;

namespace NpcChat.Backend.Validation
{
    public class SpellCheck
    {
        private static IReadOnlyDictionary<string, WordList> m_dictionary;
        private static readonly string[] s_acceptedFormats = { ".dic", ".aff" };

        static SpellCheck()
        {
            LoadSpellingData();
        }

        /// <summary>
        /// Load the spelling dictionaries and initialise the Hunspell spelling engine
        /// </summary>
        private static void LoadSpellingData()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                string dictionaryDir = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SpellCheck");
                if (!Directory.Exists(dictionaryDir))
                {
                    m_dictionary = new Dictionary<string, WordList>();
                    return;
                }

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
                ConcurrentDictionary<string, WordList> saveDictionary = new ConcurrentDictionary<string, WordList>();

                Parallel.ForEach(filePair, (pair, state) =>
                {
                    if (pair.Value[0] == null) return;

                    try
                    {
                        WordList dictionary = WordList.CreateFromFiles(pair.Value[0], pair.Value[1]);
                        saveDictionary.TryAdd(pair.Key, dictionary);

                        Logging.Logger.Log(LogLevel.Info, $"Imported '{pair.Key}' language dictionary");
                    }
                    catch (Exception ex)
                    {
                        Logging.Logger.Log(LogLevel.Error, ex,
                            $"Unable to initialize spell check directory ('{pair.Key}')");
                    }
                });
                m_dictionary = new Dictionary<string, WordList>(saveDictionary);
            }
            finally
            {
                timer.Stop();
                Logging.Logger.Log(LogLevel.Info, $"{m_dictionary.Count} Spelling dictionaries imported, took {timer.Elapsed.TotalSeconds} seconds");
            }
        }

        /// <summary>
        /// Perform spell check on the text from the element
        /// </summary>
        /// <param name="element">element to check</param>
        /// <param name="currentLanguage">language code to use spell check for</param>
        /// <param name="changedCallback">optional callback for text changes from dialog Elements</param>
        public static IEnumerable<Inline> CheckElement(IDialogElement element, string currentLanguage = "en_gb", Action<object, PropertyChangedEventArgs> changedCallback = null)
        {
            List<Inline> paragraphRuns = new List<Inline>();

            if (GeneralPreference.Instance.EnableSpellCheck && m_dictionary.ContainsKey(currentLanguage))
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

                        paragraphRuns.Add(new Bold(new SpellingOptions(word, element, suggestions.Except(new[] { word }), changedCallback)));
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
                    paragraphRuns.Add(new EditBlock(sentence, element, changedCallback));
                }
            }
            else
            {
                paragraphRuns.Add(new EditBlock(element.Text, element, changedCallback));
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
