using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Project: Counting Valleys
 * Author: Nick Steiner
 * Date: 12/15/19
 * Source: https://www.hackerrank.com/challenges/counting-valleys/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=warmup
 * 
 * Description:
 * Gary is an avid hiker. He tracks his hikes meticulously, paying close attention to small details like topography. 
 * During his last hike he took exactly n steps. For every step he took, he noted if it was an uphill, U, or a downhill, D step. 
 * Gary's hikes start and end at sea level and each step up or down represents a 1 unit change in altitude. We define the following terms:
 * - A mountain is a sequence of consecutive steps above sea level, starting with a step up from sea level and ending with a step down to sea level.
 * - A valley is a sequence of consecutive steps below sea level, starting with a step down from sea level and ending with a step up to sea level.
 * Given Gary's sequence of up and down steps during his last hike, find and print the number of valleys he walked through.
 * For example, if Gary's path is s = [DDUUUUDD], he first enters a valley 2 units deep. 
 * Then he climbs out an up onto a mountain 2 units high. Finally, he returns to sea level and ends his hike.
 * 
 * countingValleys has the following parameter(s):
 * n: the number of steps Gary takes
 * s: a string describing his path
 */
namespace CountingValleys
{
    class Program
    {
        static int countingValleys(int n, string s)
        {
            int valleys = 0;
            int pos = 0;
            for(int i = 0; i < n; i++)
            {
                if (s[i] == 'U')
                {
                    pos++;
                    if (pos == 0)
                        valleys++;
                }
                else if (s[i] == 'D')
                    pos--;
                else
                {
                    Console.WriteLine("\nCharacter '" + s[i] + "' is not a valid character. \nPlease only enter U's and D's");
                    return 0;
                }
            }
            return valleys;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of steps taken: ");
            int n = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nEnter the string of steps taken where 'U' is up and 'D' is down" +
                "\nExample: 'UDDDUDUU': ");

            string s = Console.ReadLine();

            Console.WriteLine("\nValleys traversed: " + countingValleys(n,s) + "\n");
        }
    }
}
