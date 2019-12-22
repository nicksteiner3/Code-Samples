using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Title: Minimum Swaps
 * Author: Nick Steiner
 * Date: 12/21/2019
 * Source: https://www.hackerrank.com/challenges/minimum-swaps-2/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=arrays
 * 
 * Description: 
 * You are given an unordered array consisting of consecutive integers without any duplicates. 
 * You are allowed to swap any two elements. You need to find the minimum number of swaps required to sort the array in ascending order.
 */
namespace Minimum_Swaps
{
    class Program
    {
        static void swap(int[] arr, int a, int b)
        {
            int temp = arr[a];
            arr[a] = arr[b];
            arr[b] = temp;
        }

        static int minimumSwaps(int[] arr)
        {
            int swaps = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == i + 1)
                    continue;

                swap(arr, i, arr[i] - 1);
                i = 0;
                swaps++;
            }
            return swaps;
        }

        static void Main(string[] args)
        {
            int n = Convert.ToInt32(Console.ReadLine());

            int[] arr = Array.ConvertAll(Console.ReadLine().Split(' '), arrTemp => Convert.ToInt32(arrTemp));

            Console.WriteLine(minimumSwaps(arr));
        }
    }
}
