using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Title: Ransom Note - Hash Tables
 * Author: Nick Steiner
 * Date: 12/24/2019
 * Source: https://www.hackerrank.com/challenges/ctci-ransom-note/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=dictionaries-hashmaps
 * 
 * Description: 
 * Harold is a kidnapper who wrote a ransom note, but now he is worried it will be traced back to him through his handwriting. 
 * He found a magazine and wants to know if he can cut out whole words from it and use them to create an untraceable replica of his ransom note. 
 * The words in his note are case-sensitive and he must use only whole words available in the magazine. 
 * He cannot use substrings or concatenation to create the words he needs.
 * Given the words in the magazine and the words in the ransom note, print Yes if he can replicate his ransom note exactly using whole words from the magazine; 
 * otherwise, print No.
 * For example, the note is "Attack at dawn". The magazine contains only "attack at dawn". 
 * The magazine has all the right words, but there's a case mismatch. The answer is No. 
 * 
 * Parameters:
 * - magazine: an array of strings, each word in the magazine.
 * - note: an array of strings, each a word in the ransom note.
 * 
 * Constraints: 
 * 1 <= m, n <= 30000
 * 1 <= |magazine[i]|, |note[i]| <= 5
 * Each word consists of English alphabetic letters (i.e., a to z and A to Z)
 *
 * Input: 
 * The first line contains two space-separated integers, m and n, the numbers of words in the magazine and the note..
 * The second line contains m space-separated strings, each magazine[i].
 * The third line contains n space-separated strings, each note[i].
 *
 * Output:
 * "Yes" if he can use the magazine to create an untraceable replica of his ransom note. 
 * "No" otherwise.
 * 
 * Sample Input: 
 * 6 4
 * give me one grand today night
 * give one grand today
 * 
 * Sample Output: 
 * Yes
 */
namespace RansomNote
{
    class Program
    {
        static void checkMagazine(string[] magazine, string[] note)
        {
            // Stores all strings in the magazine and the amount of times they appear 
            Dictionary<string, int> dict = new Dictionary<string, int>();

            // Fill the dictionary with magazine strings. 
            for(int i = 0; i < magazine.Length; i++)
            {
                // Increment count if string is already in the dictionary
                if (dict.ContainsKey(magazine[i]))
                    dict[magazine[i]]++;
                // Add string to the dictionary if not already added
                else
                    dict.Add(magazine[i], 1);
            }

            // Check strings in note against strings in magazine (dictionary)
            for(int i = 0; i < note.Length; i++)
            {
                // Check that the dictionary contains the string
                //  decrement or return "No" if the string's count is 0
                if (dict.ContainsKey(note[i]))
                {
                    if (dict[note[i]] > 0)
                        dict[note[i]]--;
                    else
                    {
                        Console.WriteLine("No");
                        return;
                    }                        
                }
                // Return "No" if string is not in the dictionary.
                else
                {
                    Console.WriteLine("No");
                    return;
                }

            }

            // Return yes if each word was in the dictionary (Magazine)
            Console.WriteLine("Yes");
        }

        static void Main(string[] args)
        {
            // "Yes" test case
            string[] magazine = new string[6];
            magazine[0] = "give";
            magazine[1] = "me";
            magazine[2] = "one";
            magazine[3] = "grand";
            magazine[4] = "today";
            magazine[5] = "night";

            // "No" test case
            //string[] magazine = new string[5];
            //magazine[0] = "me";
            //magazine[1] = "one";
            //magazine[2] = "grand";
            //magazine[3] = "today";
            //magazine[4] = "night";

            string[] note = new string[4];
            note[0] = "give";
            note[1] = "one";
            note[2] = "grand";
            note[3] = "today";

            checkMagazine(magazine, note);
        }
    }
}
