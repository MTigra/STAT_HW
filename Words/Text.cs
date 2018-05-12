using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;

namespace CoolSR
{
    public class Text
    {
        private string content;

        private List<string> Words
        {
            get
            {
                List<string> words = new List<string>();
                Regex rex = new Regex(@"(?:^|\s)(\S+)");
                var matches = rex.Matches(content);
                for (int i = 0; i < matches.Count; i++)
                {
                    words.Add(matches[i].Groups[0].Value);
                }
                return words;
            }

        }

        //Произвольное слово состоит только из букв и цифр.
        private List<string> NormalWords
        {
            get
            {
                List<string> normwords = new List<string>();
                Regex rex = new Regex(@"^\w+$");
                for (int i = 0; i < Words.Count; i++)
                {
                    if(rex.IsMatch(Words[i])) normwords.Add(Words[i]);
                }
                return normwords;
            }
        }

        private List<string> RealWords
        {
            get
            {
                List<string> realwords = new List<string>();
                Regex rex = new Regex(@"^[^\d\W]+$");
                for (int i = 0; i < Words.Count; i++)
                {
                    if (rex.IsMatch(Words[i])) realwords.Add(Words[i]);
                }
                return realwords;
            }
        }

        private List<string> CapitalWords
        {
            get
            {
                List<string> capwords = new List<string>();
                Regex rex = new Regex(@"^[[:upper:]]?[[:lower:]]+$");
                for (int i = 0; i < Words.Count; i++)
                {
                    if (rex.IsMatch(Words[i])) capwords.Add(Words[i]);
                }
                return capwords;
            }
        }

        public List<string> LongestWords
        {
            get
            {
                List<string> longestWords = new List<string>();
                int max = Words.Max(x => x.Length);
                for (int i = 0; i < Words.Count; i++)
                {
                 if(Words[i].Length==max) longestWords.Add(Words[i]);
                }
                return longestWords;
            }
        }
        public int LongestWordLength {
            get { return Words.Max(x => x.Length); }
        }


        public Text(string content)
        {
            this.content = content;
        }


        public void DeleteDupSpaces()
        {
            Regex rex = new Regex(@"\s\s+");
            rex.Replace(content, " ");

        }
    }
}