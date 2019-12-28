using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Title: Frequency Queries
 * Author: Nick Steiner
 * Date: 12/28/2019
 * Source: https://www.hackerrank.com/challenges/frequency-queries/problem?h_l=interview&playlist_slugs%5B%5D=interview-preparation-kit&playlist_slugs%5B%5D=dictionaries-hashmaps
 * 
 * Description:
 * You are given q queries. Each query is of the form two integers described below:
 * - 1 x : Insert x in your data structure.
 * - 2 y : Delete one occurence of y from your data structure, if present.
 * - 3 z : Check if any integer is present whose frequency is exactly . If yes, print 1 else 0.
 * 
 * The queries are given in the form of a 2-D array queries of size q where queries[i][0] contains the operation, and queries[i][1] contains the data element.
 * 
 * It must return an array of integers where each element is a 1 if there is at least one element value with the queried number of occurrences in the current array, or 0 if there is not.

 * freqQuery has the following parameter(s):
 * - queries: a 2-d array of integers
 * 
 * Sample Input:
 * 8
 * 1 5
 * 1 6
 * 3 2
 * 1 10
 * 1 10
 * 1 6 
 * 2 5
 * 3 2
 * 
 * Resulting Output:
 * 0
 * 1
 */
namespace FrequencyQueries
{
    class Program
    {
        static List<int> freqQuery(List<List<int>> queries)
        {
            List<int> result = new List<int>();                            // Return this
            int resultIndex = 0;                                           // For traversing the index of the result array
            Dictionary<int, int> dict = new Dictionary<int, int>();        // Contains what integers have been added as their key with their count as the value
            Dictionary<int, int> reverseDict = new Dictionary<int, int>(); // Contains the reverse of 'dict' for fast searching in case 3
            int value;

            // loop queries with proper case handling for cases 1, 2, and 3
            for (int i = 0; i < queries.Count; i++)
            {
                // avoids looking up this value multiple times per iteration
                value = queries[i][1]; 

                switch (queries[i][0]) 
                {
                    // Add to dict or increment count in dict
                    // Add the reverse to reverseDict
                    case 1:
                        if (dict.ContainsKey(value))
                        {
                            int amount = dict[value];

                            dict[value]++;

                            if (reverseDict.ContainsKey(amount))
                                reverseDict[amount]--;

                            amount++;
                            
                            if (reverseDict.ContainsKey(amount))
                                reverseDict[amount]++;
                            else
                                reverseDict.Add(amount, 1);
                        }
                        else
                        {
                            dict.Add(value, 1);

                            if (reverseDict.ContainsKey(1))
                                reverseDict[1]++;
                            else
                                reverseDict.Add(1, 1);
                        }
                        break;
                    // Decrement count in dict
                    // Keep track of reverse in reverseDict
                    case 2:
                        if (dict.ContainsKey(value) && dict[value] > 0)
                        {
                            reverseDict[dict[value]]--;

                            dict[value]--;

                            if(reverseDict.ContainsKey(dict[value]))
                                reverseDict[dict[value]]++;
                            else
                                Console.WriteLine("test");
                        }
                        break;
                    // Use reverse dict to quickly check the proper value
                    case 3:
                        if (reverseDict.ContainsKey(value) && reverseDict[value] > 0)
                            result.Insert(resultIndex, 1);
                        else
                            result.Insert(resultIndex, 0);
                        resultIndex++;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the number of queries followed by enter, followed by each query separated by enter");
            Console.WriteLine("\nExample: \n4 \n3 4 \n2 1003 \n1 16 \n3 1\n");
            int q = Convert.ToInt32(Console.ReadLine().Trim());

            List<List<int>> queries = new List<List<int>>();

            for (int i = 0; i < q; i++)
            {
                queries.Add(Console.ReadLine().TrimEnd().Split(' ').ToList().Select(queriesTemp => Convert.ToInt32(queriesTemp)).ToList());
            }

            List<int> ans = freqQuery(queries);

            Console.WriteLine("\nResult:");
            for(int i = 0; i < ans.Count; i++)
                Console.WriteLine(ans[i]);
        }
    }
}
