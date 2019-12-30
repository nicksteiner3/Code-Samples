using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Nick Steiner
 * 12/14/19
 * Jumping on the Clouds problem from Hackerrank.com
 * 
 * Source: https://www.hackerrank.com/challenges/jumping-on-the-clouds/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=warmup
 * 
 * Description:
 * Emma is playing a new mobile game that starts with consecutively numbered clouds. 
 * Some of the clouds are thunderheads and others are cumulus. 
 * She can jump on any cumulus cloud having a number that is equal to the number of the current cloud plus 1 or 2. 
 * She must avoid the thunderheads. 
 * Determine the minimum number of jumps it will take Emma to jump from her starting postion to the last cloud. 
 * It is always possible to win the game.
 * For each game, Emma will get an array of clouds numbered  if they are safe or  if they must be avoided. For example, c = [0,1,0,0,0,1,0] indexed from 0 to 6. 
 * The number on each cloud is its index in the list so she must avoid the clouds at indexes 1 and 5. She could follow the following two paths: 0->2->4->6
 * or 0->2->3->4->6. The first path takes 3 jumps while the second takes 4.
 */
namespace JumpingOnTheClouds
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the number of clouds you want to process");
            int N = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter each cloud followed by pressing enter");
            Console.WriteLine("A safe cloud is a 0, and a cloud that must be avoided is a 1");

            int[] arr = new int[N];
            for (int i = 0; i < N; i++) {
               arr[i] = Convert.ToInt32(Console.ReadLine());
            }
            Console.WriteLine("Minimum amount of jumps: " + jumpingOnClouds(arr));
        }

        static int jumpingOnClouds(int[] c)
        {
            int jumps = 0;
            int current = 0;
            int end = c.Length-1;

            while (current != end)
            {
                if (current == end - 1)
                {
                    jumps++;
                    break;
                }
                if (c[current + 2] == 0)
                    current += 2;
                else
                    current++;

                jumps++;
            }
            return jumps;
        }
    }
}
