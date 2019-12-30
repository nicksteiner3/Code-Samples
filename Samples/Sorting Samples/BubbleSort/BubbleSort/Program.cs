using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Title: Bubble Sort
 * Author: Nick Steiner
 * Date: 12/29/2019
 * Source: https://www.hackerrank.com/challenges/ctci-bubble-sort/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=sorting
 * 
 * Description:
 * Sort an array using the bubble sort method.
 * Then print out 3 lines:
 * - How many swaps took place
 * - The first element in the sorted array
 * - The last element in the sorted array
 */
namespace BubbleSort
{
    class Program
    {
        static void swap(int[] arr, int a, int b)
        {
            int temp = arr[a];
            arr[a] = arr[b];
            arr[b] = temp;
        }

        static void countSwaps(int[] a)
        {
            int swaps = 0;
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a.Length - 1; j++)
                {
                    // Swap adjacent elements if they are in decreasing order
                    if (a[j] > a[j + 1])
                    {
                        swap(a, j, j + 1);
                        swaps++;
                    }
                }
            }

            Console.WriteLine("\nArray is sorted in " + swaps + " swaps.");
            Console.WriteLine("First Element: " + a[0]);
            Console.WriteLine("Last Element: " + a[a.Length - 1]);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of elements in your array to be sorted:");
            int n = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("\nEnter each value in your array, separating each element by a space: ");
            int[] a = Array.ConvertAll(Console.ReadLine().Split(' '), aTemp => Convert.ToInt32(aTemp))
            ;
            countSwaps(a);

            Console.WriteLine("");
        }
    }
}
