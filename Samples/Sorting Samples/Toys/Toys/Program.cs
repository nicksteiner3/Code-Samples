using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toys
{
    class Program
    {
        static void swap(int[] arr, int a, int b)
        {
            int temp = arr[a];
            arr[a] = arr[b];
            arr[b] = temp;
        }

        // Complete the maximumToys function below.
        static int maximumToys(int[] prices, int k)
        {
            for (int i = 0; i < prices.Length; i++)
            {
                for (int j = 0; j < prices.Length - 1; j++)
                {
                    // Swap adjacent elements if they are in decreasing order
                    if (prices[j] > prices[j + 1])
                    {
                        swap(prices, j, j + 1);
                    }
                }
            }
            int remaining = k;
            int totalToys = 0;
            for(int i = 0; i < prices.Length; i++)
            {
                remaining -= prices[i];
                if (remaining < 0)
                {
                    remaining += prices[i];
                    break;
                }
                else
                    totalToys++;
            }

            return totalToys;
        }

        static void Main(string[] args)
        {
            string[] nk = Console.ReadLine().Split(' ');

            int n = Convert.ToInt32(nk[0]);

            int k = Convert.ToInt32(nk[1]);

            int[] prices = Array.ConvertAll(Console.ReadLine().Split(' '), pricesTemp => Convert.ToInt32(pricesTemp))
            ;
            int result = maximumToys(prices, k);

            Console.WriteLine(result);
        }
    }
}
