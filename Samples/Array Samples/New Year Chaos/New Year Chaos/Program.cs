using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Author: Nick Steiner
 * Date: 12/20/2019
 * Title: New Year Chaos
 * Source: https://www.hackerrank.com/challenges/new-year-chaos/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=arrays
 * 
 * Description: 
 * It's New Year's Day and everyone's in line for the Wonderland rollercoaster ride! 
 * There are a number of people queued up, and each person wears a sticker indicating their initial position in the queue. 
 * Initial positions increment by 1 from 1 at the front of the line to  at the back.
 * Any person in the queue can bribe the person directly in front of them to swap positions. 
 * If two people swap positions, they still wear the same sticker denoting their original places in line. 
 * One person can bribe at most two others. For example, if n=8 and Person 5 bribes Person 4, the queue will look like this: [1,2,3,5,4,6,7,8].
 * Fascinated by this chaotic queue, you decide you must know the minimum number of bribes that took place to get the queue into its current state!
 * 
 * minimumBribes has the following parameter(s):
 * - q: an array of integers
 * - t: Amount of bribes allowe per person
 */
namespace New_Year_Chaos
{
    class Program
    {
        static void minimumBribes(int[] q, int t)
        {
            int totalBribes = 0;

            int expectedFirst = 1;
            int expectedSecond = 2;
            int expectedThird = 3;

            for (uint i = 0; i < q.Length; ++i)
            {
                if (q[i] == expectedFirst)
                {
                    expectedFirst = expectedSecond;
                    expectedSecond = expectedThird;
                    ++expectedThird;
                }
                else if (q[i] == expectedSecond)
                {
                    ++totalBribes;
                    expectedSecond = expectedThird;
                    ++expectedThird;
                }
                else if (q[i] == expectedThird)
                {
                    totalBribes += 2;
                    ++expectedThird;
                }
                else
                {
                    Console.WriteLine("\nToo chaotic");
                    return;
                }
            }

            Console.WriteLine("\nMinimum total bribes required: " + totalBribes);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of bribes allowed");
            int t = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nEnter the number of people in line");
            int n = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nEnter the final order of the line");
            string orderRaw = Console.ReadLine();

            int[] order = new int[n];

            int index = 0;
            for(int i = 0; i < orderRaw.Length; i++)
            {
                if (orderRaw[i] == ' ')
                    continue;

                order[index] = Convert.ToInt32(orderRaw[i] - '0');
                index++;
            }

            minimumBribes(order, t);
        }
    }
}
