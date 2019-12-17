using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Title: 2D Arrays - Hourglass
 * Author: Nick Steiner
 * Date: 12/16/19
 * Source: https://www.hackerrank.com/challenges/2d-array/problem?h_l=interview&page=1&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=arrays
 * 
 * Description:
 * Given a 6x6 2D array
 * We define an hourglass in A to be a subset of values with indices falling in this pattern in arr's graphical representation:
 * a b c
 *   d
 * e f g
 * 
 * There are 16 hourglasses in arr, and an hourglass's sum is the sum of an hourglass' values. 
 * Calculate the hourglass sum for every hourglass in arr, then print the maximum hourglass sum.
 * 
 * Constraints:
 * - arr[i][j] is between -9 and 9 inclusive
 * - i and j are between 0 and 5 inclusive
 */
namespace _2D_Array___DS
{
    class Program
    {
        static int hourglassSum(int[][] arr)
        {
            // Minimum possible Sum (-9*7)
            int ret = -63;

            int i = 0;
            int j = 0;
            int sum = 0;

            // Loope through each hourglass row (An hourglass can't start at position 5 or 6)
            while (i < 4)
            {
                // And each hourglass column
                while (j < 4)
                {
                    // Getting the sum from the hourglass at that position
                    sum = arr[i][j] + arr[i][j + 1] + arr[i][j + 2] + arr[i + 1][j + 1] + arr[i + 2][j] + arr[i + 2][j + 1] + arr[i + 2][j + 2];

                    // If the sum of this hourglass is greater than the current greatest hourglass sum, then set it as the return value. 
                    if (sum > ret)
                        ret = sum;

                    j++;
                }

                i++;
                j = 0;
            }

            return ret;
        }

        static void Main(string[] args)
        {
            int[][] arr = new int[6][];

            Console.WriteLine("Enter your 2D array one line at a time, pressing enter after each line: ");
            for (int i = 0; i < 6; i++)
            {
                arr[i] = Array.ConvertAll(Console.ReadLine().Split(' '), arrTemp => Convert.ToInt32(arrTemp));
            }

            Console.WriteLine(hourglassSum(arr));
        }
    }
}
