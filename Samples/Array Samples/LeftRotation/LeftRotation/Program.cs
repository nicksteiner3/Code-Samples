using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Title: Array - Left Rotation
 * Author: Nick Steiner
 * Date: 12/19/2019
 * Source: https://www.hackerrank.com/challenges/ctci-array-left-rotation/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=arrays
 * 
 * Description: 
 * A left rotation operation on an array shifts each of the array's elements 1 unit to the left. 
 * For example, if 2 left rotations are performed on array [1,2,3,4,5], then the array would become [3,4,5,1,2].
 * Given an array a of n integers and a number, d, perform d left rotations on the array. 
 * Return the updated array to be printed as a single line of space-separated integers.
 */
namespace LeftRotation
{
    class Program
    {
        static int[] rotLeft(int[] a, int d)
        {
            // Length of array to avoid looking up the value multiple times. 
            int len = a.Length;

            // The final array
            int[] newArr = new int[len];

            // The scanning index that starts at d
            int index = d;

            // Set the new array index by index, moving the index to the right unless it has reached it's max distance to the right. 
            //  in that case, move the index to position 0
            for (int i = 0; i < len; i++)
            {
                newArr[i] = a[index];

                if (index == len - 1)
                    index = 0;
                else
                    index++;
            }

            return newArr;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the length of your array");
            int len = Convert.ToInt32(Console.ReadLine());

            int[] arr = new int[len];
            int[] newArr = new int[len];

            Console.WriteLine("\nEnter the values of your array one line at a time:");
            for(int i = 0; i < len; i++)
            {
                arr[i] = Convert.ToInt32(Console.ReadLine());
            }

            Console.WriteLine("\nEnter the amount to shift your array left");

            int amount = Convert.ToInt32(Console.ReadLine());

            newArr = rotLeft(arr, amount);

            Console.Write("\nThe Array shifted to the left by " + amount + " is:\n[");
            for(int j = 0; j < newArr.Length-1; j++)
                Console.Write(newArr[j] + ", ");

            Console.Write(newArr[len-1] + "]\n\n");
        }
    }
}
