using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Title: Repeated String
 * Author: Nick Steiner
 * Date: 12/14/19
 * Source: https://www.hackerrank.com/challenges/repeated-string/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=warmup
 * 
 * Description:
 * Lilah has a string, , of lowercase English letters that she repeated infinitely many times.
 * Given an integer, , find and print the number of letter a's in the first  letters of Lilah's infinite string.
 * For example, if the string s='abcac' and n = 10, the substring we consider is abcacabcac, the first 10 characters of her infinite string. 
 * There are 4 occurrences of a in the substring.
 * Complete the repeatedString function in the editor below. It should return an integer representing the number of occurrences of a in the prefix of length n
 *   in the infinitely repeating string.
 * 
 * repeatedString has the following parameter(s):
 * s: a string to repeat
 * n: the number of characters to consider
 */
namespace Repeated_String
{
    class Program
    {
        static long repeatedString(string s, long n)
        {
            int len = s.Length;

            if (len == 1) {
                if (s[0] == 'a')
                    return n;
                else
                    return 0;
            }

            int numOfAs = 0;

            for(int i = 0; i < len; i++)
            {
                if (s[i] == 'a')
                    numOfAs++;
            }

            long ret = n / len;

            ret *= numOfAs;

            long rem = n % len;

            for(int i = 0; i < rem; i++)
            {
                if (s[i] == 'a')
                    ret++;
            }

            return ret;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Provide the string s");
            string s = Console.ReadLine();
            Console.WriteLine("Provide the length of the repeating string n");
            long n = Convert.ToInt64(Console.ReadLine());

            Console.WriteLine("\n" + "Number of a's is: " + repeatedString(s,n));
        }
    }
}
