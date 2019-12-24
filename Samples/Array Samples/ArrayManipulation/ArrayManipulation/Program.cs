using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Title: Array Manipulation
 * Author: Nick Steiner
 * Date: 12/23/2019
 * Source: https://www.hackerrank.com/challenges/crush/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=arrays
 * 
 * Description: 
 * Starting with a 1-indexed array of zeros and a list of operations, for each operation add a value to each of the array element between two given indices, inclusive. 
 * Once all operations have been performed, return the maximum value in your array.
 * 
 * For example, the length of your array of zeros n = 10. Your list of queries is as follows:
 * a b k
 * 1 5 3
 * 4 8 7
 * 6 9 1
 * 
 * Add the values of k between the indices a and b inclusive:
 * index->	 1 2 3  4  5 6 7 8 9 10
	        [0,0,0, 0, 0,0,0,0,0, 0]
	        [3,3,3, 3, 3,0,0,0,0, 0]
	        [3,3,3,10,10,7,7,7,0, 0]
	        [3,3,3,10,10,8,8,8,1, 0]

 * The largest value is 10 after all operations are performed.
 */
namespace ArrayManipulation
{
    class Program
    {
        static long arrayManipulation(int n, int[][] queries)
        {
            long max = 0;
            long[] arr = new long[n];
            long numQueries = queries.GetLength(0);
            long first;
            long last;
            long summand;

            /*
             * Loop through the given queries:
             * add the summand to the given start index of 'arr'. 
             * subtract the summand from the given end index of 'arr'. (If the given end idex is the last index, just ignore and continue)
             */
            for (int i = 0; i < numQueries; i++)
            {
                first = queries[i][0]-1;
                last = queries[i][1];
                summand = queries[i][2];

                arr[first] += summand;
                if(last < n)
                    arr[last] -= summand;
            }

            long sum = 0;

            /*
             * Loop through arr adding and subtracting the sums calculated above. 
             * At each step, we check for the max sum which we return after the loop ends.
             */ 
            for(int i = 0; i < n; i++)
            {
                sum += arr[i];
                if (sum > max)
                    max = sum;
            }
            return max;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("n is the size of the array.");
            Console.WriteLine("m is the number of operations.");
            Console.WriteLine("Enter n and m separated by a space:");

            string[] nm = Console.ReadLine().Split(' ');

            int n = Convert.ToInt32(nm[0]);

            int m = Convert.ToInt32(nm[1]);

            int[][] queries = new int[m][];

            Console.WriteLine("Enter operations according to this example, where the first index is 1, the second index is 2, \nand the value to add is 100: ");
            Console.WriteLine("1 2 100");
            Console.WriteLine("You must enter the amount of lines you specified as m above:");
            for (int i = 0; i < m; i++)
            {
                queries[i] = Array.ConvertAll(Console.ReadLine().Split(' '), queriesTemp => Convert.ToInt32(queriesTemp));
            }

            long result = arrayManipulation(n, queries);
            Console.WriteLine(result);
        }
    }
}
