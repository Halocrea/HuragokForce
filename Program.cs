using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using NTextCat;

/*
 * STUPID BRUTEFORCING OF THE SUBGLYPHS
 * 
 * numbers in "sequence" are just meant to be the subglyphs and not to point at a specific order[1]. 
 * The number of variation for 10 characters and 26 choices each is 19,275,223,968,000 (and that's too much).
 * 
 * As computations of all variations without repetition means factorial complexity, 
 * we needed to narrow down a few things:
 *      1. We're going to assume that the subglyphs are roman letters we can "translate" 
 *         such as "subglyph 1 = X, subglyph 2 = N" and so on.
 *         
 *      2. We're assuming they are english words.
 *      
 *      3. Given that the subglyph 2 is by far the most common and repeated 6 times, and looking at 
 *         frequency analysis of the letters in english vocabulary[3], we're going to assume this 
 *         is the "E" letter.
 *         
 *      4. Our alphabet is going to be amputated of the 4 less commonly used letters in english vocabulary: 
 *         Q, J, Z, X ( < 0.2% frequency)[3].
 *         
 *      5. Most risky asumption: we're going to assume that the subglyph 0, being between two "E"s, is a "Y". 
 *         It's probably stupid, but we need to narrow those numbers down to something almost reasonable.
 *
 * With all those base criterias, we end up with 8 characters that can be any of 20 letters, which makes 
 * 5,079,110,400 possibilities.
 * 
 * I ran a test (in debug and for one minute, so give or take) locally, and I had on average 
 * 150,000 computed variations every minute. 
 * Based on that observation, computing and testing 5,079,110,400 possibilities may take around 23 days 
 * on a computer running the program 24/7.
 * 
 * Once the program computed a variation, we'll get it tested with a library that will determine the language 
 * that may be used in it[4]. 
 * The issue is: the library (and many others, this one is actually performing pretty OK) is obviously struggling 
 * with the lack of space in the sentence.
 * 
 * But choices need to be made: I ran some tests, and the library gives a "level of confidence": 
 *      - the lower that number is, the most confident the program is about its deduction, 
 *      - trying 19 character long, non-spaced, english sentences gave me levels between 3953 and 3962,
 *      - based on this, the program will only store english sentences with a level of confidence equal 
 *        or smaller than 3964.
 * 
 * 
 * Annotations:
 * [1] subglyphs and their designated number: https://imgur.com/a/mkxUwqm 
 * [2] calculating the number of possibilities: https://www.hackmath.net/en/calculator/combinations-and-permutations?n=26&k=10&order=1&repeat=0
 * [3] frequency of letters in english vocabulary: https://www.lexico.com/explore/which-letters-are-used-most 
 * [4] NtextCat: https://github.com/ivanakcheurov/ntextcat 
 * 
 */
namespace HuragokForce
{
    class Program
    {
        private static float CONFIDENCE_THRESHOLD   = 3964; // 3964 or lower means pretty good chances that the permutation is of interest
        private static char[] ALPHABET              = new char[] { 'A', 'B', 'C', 'D', 'F', 'G', 'H', 'I', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'V', 'W' }; 
        private static string[] SEQUENCE            = new string[] { "0", "1", "E", "2", "3", "0", "4", "5", "E", "6", "4", "2", "E", "E", "6", "7", "E", "Y", "E" };
        private static string FMT                   = "0000000000000";

        static void Main(string[] args)
        {
            Console.WriteLine("Program started at {0}", DateTime.Now.ToString());
            var factory     = new RankedLanguageIdentifierFactory();
            var identifier  = factory.Load("Core14.profile.xml");

            var result      = Permute(ALPHABET, 8);
            int count       = 0;

            foreach (var perm in result)
            {
                count++;
                int index           = 0;
                string[] currentSeq = (string[])SEQUENCE.Clone();

                foreach (var c in perm)
                {
                    currentSeq = currentSeq.Select(x => x.Replace(index.ToString()[0], c)).ToArray();
                    index++;
                }
                DetectLanguage(identifier, String.Join(null, currentSeq).ToLower(), count);
            }
            Console.WriteLine("Finished processing at {0}", DateTime.Now.ToString());
            Console.ReadKey();
        }
        public static IEnumerable<T> AllExcept<T>(IEnumerable<T> input, int indexToSkip)
        {
            int index = 0;
            foreach (T item in input)
            {
                if (index != indexToSkip) yield return item;
                index += 1;
            }
        }

        public static IEnumerable<T> Concat<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T item in a) { yield return item; }
            foreach (T item in b) { yield return item; }
        }

        public static void DetectLanguage(RankedLanguageIdentifier identifier, string sequence, int index)
        {
            var languages            = identifier.Identify(sequence.ToLower());
            var mostCertainLanguage  = languages.FirstOrDefault();

            if (mostCertainLanguage != null && mostCertainLanguage.Item1.Iso639_3 == "eng" && mostCertainLanguage.Item2 <= CONFIDENCE_THRESHOLD)
                SaveToFile(sequence, mostCertainLanguage.Item2, index);
        }

        public static void SaveToFile (string sequence, double levelOfConfidence, int index)
        {
            string toSave = 
                "[ " + DateTime.Now.ToString() + " - #" + index.ToString(FMT) + " - Confidence: " + levelOfConfidence.ToString("0000.00") + " ] " + String.Join(null, sequence);

            try
            {
                StreamWriter sw = new StreamWriter("E:\\Test1.txt", true, Encoding.UTF8);
                sw.WriteLine(toSave.ToUpper());
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("SAVED => {0}", toSave.ToUpper());
            }
        }
        public static IEnumerable<IEnumerable<T>> Permute<T>(IEnumerable<T> list, int count)
        {
            if (count == 0)
            {
                yield return new T[0];
            }
            else
            {
                int startingElementIndex = 0;
                foreach (T startingElement in list)
                {
                    IEnumerable<T> remainingItems = AllExcept(list, startingElementIndex);

                    foreach (IEnumerable<T> permutationOfRemainder in Permute(remainingItems, count - 1))
                    {
                        yield return Concat<T>(
                            new T[] { startingElement },
                            permutationOfRemainder);
                    }
                    startingElementIndex += 1;
                }
            }
        }
    }
}
