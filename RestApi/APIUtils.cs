using RestApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApi
{
    public static class APIUtils
    {
        //public static int CalculateScore(decimal v)
        //{
        //    return Convert.ToInt32(Math.Round(v, 4) * 1000);
        //}

        public static string GetRandomWord()
        {
            Random r = new Random();
            string word;

            using (StreamReader sr = new StreamReader("./givenWords.txt"))
            {
                var wordArr = sr.ReadToEnd().Split(',');
                word = wordArr[r.Next(0, wordArr.Length)].Trim();
            }
            return word;
        }

        public static string GetRandomWordHardcoded()
        {
            Random r = new Random();
            string[] givenWordArray = new string[] { "aika", "vuosi", "kuukausi", "aurinkokunta", "kuu", "planeetta" };

            string word = givenWordArray[r.Next(givenWordArray.Length)];

            return word;
        }

        internal static string RemovePunctuation(string s)
        {

            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (char.IsLetterOrDigit(c) || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();

        }

        internal static Wordlist PoistaTurhat(Wordlist sanalista)
        {
            
            var lista=sanalista.Guesses.ToList();
            foreach (var item in sanalista.Guesses)
            {
                item.ToLower();
                if (!IsAlphaOrHyphen(item))   
                    lista.Remove(item);

               
            }
                return new Wordlist() { Guesses=lista.ToArray(), Original=sanalista.Original };

            }
        internal static bool IsAlphaOrHyphen(string s)
        {
            var letterFound = false;
            foreach (var c in s)
            {
                if (!char.IsLetter(c) && c != '-')
                {
                    return false;
                }
                else if (char.IsLetter(c))
                {
                    letterFound = true;
                }
            }
            return letterFound;
        }
    }
    }

