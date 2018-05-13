using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;

namespace CoolSR
{
    public class Text
    {
        /// <summary>
        /// Поле содержит в себе текст.
        /// </summary>
        private string content;
        /// <summary>
        /// Поле содержит в себе список слов.
        /// </summary>
        private List<string> words;
        /// <summary>
        /// Поле содержит в себе список произвольных слов.
        /// </summary>
        private List<string> normalWords;
        /// <summary>
        /// Поле содержит список настоящих слов
        /// </summary>
        private List<string> realWords;

        /// <summary>
        /// Возвращает содержимое
        /// </summary>
        public string Content => content;

        /// <summary>
        /// Возвращает количество символов.
        /// </summary>
        public int Length => content.Length;

        /// <summary>
        /// возвращает количество строк.
        /// </summary>
        public int LinesCount
        {
            get { return content.Split('\n').Length; }
        }

        #region Слово
        /// <summary>
        /// Возвращает или задает список слов.
        /// </summary>
        public List<string> Words
        {
            get
            {
                return words;
            }
            private set { words = value; }
        }

        /// <summary>
        /// Находит слова из content и возвращает в виде списка.
        /// </summary>
        /// <returns></returns>
        public List<string> GetWords()
        {
            List<string> words = new List<string>();
            Regex rex = new Regex(@"(?:^|\s)(\S+)");
            var matches = rex.Matches(content);
            for (int i = 0; i < matches.Count; i++)
            {
                words.Add(matches[i].Groups[1].Value);
            }
            return words;
        }

        /// <summary>
        /// Возвращает список из слов максимальной длины
        /// </summary>
        public List<string> LongestWords
        {
            get
            {
                List<string> longestWords = new List<string>();
                int max = Words.Max(x => x.Length);
                for (int i = 0; i < Words.Count; i++)
                {
                    if (Words[i].Length == max) longestWords.Add(Words[i]);
                }
                return longestWords;
            }
        }

        #endregion

        #region Произвольное слово

        /// <summary>
        /// Вовзращает или задает список Произвольных слов.
        /// </summary>
        public List<string> NormalWords
        {
            get { return normalWords; }
            private set { normalWords = value; }
        }
        /// <summary>
        /// Находит произвольные слова из списка слов и возвращает в виде списка.
        /// </summary>
        /// <returns></returns>
        public List<string> GetNormalWords()
        {
            List<string> normwords = new List<string>();
            Regex rex = new Regex(@"^\w+$");
            for (int i = 0; i < Words.Count; i++)
            {
                if (rex.IsMatch(Words[i])) normwords.Add(Words[i]);
            }
            return normwords;
        }
        /// <summary>
        /// Возвращает список из произвольных слов максимальной длины
        /// </summary>
        public List<string> LongestNormalWords
        {
            get
            {
                List<string> longestWords = new List<string>();
                int max = NormalWords.Max(x => x.Length);
                for (int i = 0; i < NormalWords.Count; i++)
                {
                    if (NormalWords[i].Length == max) longestWords.Add(NormalWords[i]);
                }
                return longestWords;
            }
        }
        #endregion

        #region Настоящее слово



        /// <summary>
        /// Возвращает или задает список Настоящих слов.
        /// </summary>
        public List<string> RealWords
        {
            get { return realWords; }
            private set { realWords = value; }
        }
        /// <summary>
        /// Возвращает список настоящих слов из списка слов в виде списка.
        /// </summary>
        /// <returns></returns>
        public List<string> GetRealWords()
        {
            List<string> realwords = new List<string>();
            Regex rex = new Regex(@"^[^\d\W_]+$");
            for (int i = 0; i < Words.Count; i++)
            {
                if (rex.IsMatch(Words[i])) realwords.Add(Words[i]);
            }
            return realwords;
        }

        /// <summary>
        /// Возвращает список слов максимальной длины.
        /// </summary>
        public List<string> LongestRealWords
        {
            get
            {
                List<string> longestWords = new List<string>();
                int max = RealWords.Max(x => x.Length);
                for (int i = 0; i < RealWords.Count; i++)
                {
                    if (RealWords[i].Length == max) longestWords.Add(RealWords[i]);
                }
                return longestWords;
            }
        }

        #endregion

        /// <summary>
        /// Возврщает список заглавных слов. из списка настоязих слов
        /// </summary>
        public List<string> CapitalWords
        {
            get
            {
                List<string> capwords = new List<string>();
                Regex rex = new Regex(@"^[[:upper:]]?[[:lower:]]+$");
                for (int i = 0; i < RealWords.Count; i++)
                {
                    if (rex.IsMatch(RealWords[i])) capwords.Add(RealWords[i]);
                }
                return capwords;
            }
        }
        /// <summary>
        /// Возвращает список акронимов
        /// </summary>
        public List<string> Acronyms
        {
            get
            {
                List<string> acronyms = new List<string>();
                Regex rex = new Regex(@"^(?:[A-Z]){2,}$");
                for (int i = 0; i < RealWords.Count; i++)
                {
                    if (rex.IsMatch(RealWords[i])) acronyms.Add(RealWords[i]);
                }
                return acronyms;
            }
        }

        public Text(Text text) : this(text.content)
        {

        }


        public Text(string content)
        {
            this.content = content;
            Words = GetWords();
            NormalWords = GetNormalWords();
            RealWords = GetRealWords();
            
        }

        /// <summary>
        /// Удаляет повторяющиеся непечатные символы, заменяет на один пробел
        /// </summary>
        public void DeleteDupSpaces()
        {
            Regex rex = new Regex(@"\s\s+");
            content = rex.Replace(content, " ");
            Words = GetWords();
            NormalWords = GetNormalWords();
            RealWords = GetRealWords();
        }
        /// <summary>
        /// Оставляет только настоящие слова и пробел следюущий за ним.
        /// </summary>
        public void OnlyRealWords()
        {
            string newcontent = "";
            for (int i = 0; i < RealWords.Count; i++)
            {
                newcontent += RealWords[i] + " ";
            }

            content = newcontent;
            Words = GetWords();
            NormalWords = GetNormalWords();
            RealWords = GetRealWords();
        }

       
    }
}