using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace test_git
{

    public class Node
    {

        private readonly Node[] children = new Node[26];

        public IEnumerable<KeyValuePair<Node, char>> AssignedChildren
        {
            get
            {
                for (int i = 0; i < 26; i++)
                {
                    if (children[i] != null)
                        yield return new KeyValuePair<Node, char>(children[i], (char)('a' + i));
                }
            }
        }

        public Node GetOrCreate(char c)
        {
            Node child = this[c];
            if (child == null)
                child = this[c] = new Node();
            return child;
        }

        public Node this[char c]
        {
            get { return children[c - 'a']; }
            set { children[c - 'a'] = value; }
        }

        public bool IsWordTerminator { get; set; }

    }

    public class Trie2
    {

        private readonly Node root = new Node();

        public Node NodeForWord(string word, bool createPath)
        {
            Node current = root;

            foreach (char c in word)
            {
                if (createPath)
                    current = current.GetOrCreate(c);
                else
                    current = current[c];

                if (current == null)
                    return null;
            }

            return current;
        }

        public void AddNodeForWord(string word)
        {
            Node node = NodeForWord(word, true);
            node.IsWordTerminator = true;
        }

        public bool ContainsWord(string word)
        {
            Node node = NodeForWord(word, false);
            return node != null && node.IsWordTerminator;
        }

        public List<string> PrefixedWords(string prefix)
        {
            var prefixedWords = new List<string>();
            Node node = NodeForWord(prefix, false);
            if (node == null)
                return prefixedWords;

            PrefixedWordsAux(prefix, node, prefixedWords);
            return prefixedWords;
        }

        private void PrefixedWordsAux(string word, Node node, List<string> prefixedWords)
        {
            if (node.IsWordTerminator)
                prefixedWords.Add(word);

            foreach (var child in node.AssignedChildren)
            {
                PrefixedWordsAux(word + child.Value, child.Key, prefixedWords);
            }
        }

    }


    public class AutoCompleteViewModel : INotifyPropertyChanged
    {

        private Trie2 trie = new Trie2();

        private int prefixMinSize;
        public int PrefixMinSize
        {
            get { return prefixMinSize; }
            set
            {
                if (prefixMinSize != value)
                {
                    prefixMinSize = value;
                    RaisePropertyChanged("PrefixMinSize");
                }
            }
        }

        private string calculationInfo;
        public string CalculationInfo
        {
            get { return calculationInfo; }
            set
            {
                if (calculationInfo != value)
                {
                    calculationInfo = value;
                    RaisePropertyChanged("CalculationInfo");
                }
            }
        }

        private string inputWord;
        public string InputWord
        {
            get { return inputWord; }
            set
            {
                if (inputWord != value)
                {
                    value = value.ToLower();
                    inputWord = string.Join("", value.ToCharArray().Where(c => (int)c >= (int)'a' && (int)c <= (int)'z').ToArray());
                    RaisePropertyChanged("InputWord");
                }
            }
        }

        public ObservableCollection<string> PrefixList { get; set; }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public AutoCompleteViewModel()
        {
            PrefixMinSize = 3;
            InputWord = "Adv";
            PrefixList = new ObservableCollection<string>();
            ProcessDictionaryList();
        }

        private void ProcessDictionaryList()
        {
            foreach (var word in File.ReadLines("english-words"))
            {
                trie.AddNodeForWord(word);
            }
        }

        public void SetPrefixList()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            PrefixList.Clear();
            var prefixes = GetPrefixList();

            foreach (var prefix in prefixes)
            {
                PrefixList.Add(prefix);
            }
            //RaisePropertyChanged("PrefixList"); 

            CalculationInfo = string.Format("Retrieved {0} words prefixed with {1}. Operation took {2} ms", PrefixList.Count, inputWord, stopWatch.ElapsedMilliseconds);
        }

        private List<string> GetPrefixList()
        {
            if (InputWord.Length >= PrefixMinSize)
            {
                var wordsStartingWithInputWord = trie.PrefixedWords(InputWord.ToLower());
                return wordsStartingWithInputWord;
            }
            return new List<string>();
        }
    }
}



