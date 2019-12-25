using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Title: Two Strings - Dictionaries
 * Author: Nick Steiner
 * Date: 12/25/2019
 * Source: https://www.hackerrank.com/challenges/two-strings/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=dictionaries-hashmaps
 * 
 * Description: Given two strings, determine if they share a common substring. A substring may be as small as one character.
 * For example, the words "a", "and", "art" share the common substring "a". The words "be" and "cat" do not share a substring.
 *
 * Parameters:
 * s1, s2: two strings to analyze.
 *
 * Input Format:
 * The first line contains a single integer p, the number of test cases.
 * The following  pairs of lines are as follows:
 * The first line contains string s1.
 * The second line contains string s2.
 *
 * Constraints:
 * - s1 and s2 consist of characters in the range ascii[a-z].
 * - 1 <= p <= 10
 * - 1 <= |s1|,|s2| <= 10^5
 * 
 * Output Format:
 * For each pair of strings, return YES or NO.
 *
 * Sample Input:
 * 2
 * hello
 * world
 * hi
 * world
 * 
 * Sample Output:
 * YES
 * NO
 */
namespace TwoStrings
{
    class Program
    {
        static string twoStrings(string s1, string s2)
        {
            Dictionary<char, bool> dict = new Dictionary<char, bool>();

            for(int i = 0; i < s1.Length; i++)
                if (!dict.ContainsKey(s1[i]))
                    dict.Add(s1[i], true);

            for(int i = 0; i < s2.Length; i++)
                if (dict.ContainsKey(s2[i]))
                    return "YES";

            return "NO";
        }

        static void Main(string[] args)
        {
            string s1;
            string s2;

            while(true)
            {
                Console.WriteLine("Enter the first word: ");
                s1 = Console.ReadLine();
                Console.WriteLine("\nEnter the second word: ");
                s2 = Console.ReadLine();
                Console.WriteLine("\nDo the strings contain a common substring?");
                Console.WriteLine(twoStrings(s1, s2));
                Console.WriteLine();
                Console.WriteLine("-----------------------------------");
                Console.WriteLine();
            }
        }
    }
}
