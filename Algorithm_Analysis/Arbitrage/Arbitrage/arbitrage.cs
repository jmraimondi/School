using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage
{
    class arbitrage
    {
        static void Main(string[] args)
        {
            double max = double.MaxValue;
            double[,] graph = new double[,] { { 0, Math.Log10(0.75), max}, { max, 0, Math.Log10(2.0)}, {Math.Log10(0.7), max, 0} };
            //double[,] graph = new double[,] { { 0, 0.75, max }, { max, 0, 2.0 }, { 0.7, max, 0 } };
            fw(graph);

            Console.ReadKey();
        }

        static void fw(double[,] g)
        {
            double[,] shortestGraph = new double[g.GetLength(0), g.GetLength(1)];
            int length = g.GetLength(0);
            Array.Copy(g, shortestGraph, g.Length);

            for (int k = 0; k < length; k++)
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if(shortestGraph[i,k] == double.MaxValue || shortestGraph[k,j] == double.MaxValue)
                        {
                            continue;
                        }
                        /* if (Math.Log10(shortestGraph[i, k] + shortestGraph[k, j]) < Math.Log10(shortestGraph[i, j]))
                         {
                             shortestGraph[i, j] = Math.Log10(shortestGraph[i, k] + shortestGraph[k, j]);
                         }
                         */

                        if (shortestGraph[i, k] + shortestGraph[k, j] < shortestGraph[i, j])
                        {
                            shortestGraph[i, j] = shortestGraph[i, k] + shortestGraph[k, j];
                        }
                    }
                }
            }

            printGraph(shortestGraph);
        }

        static void printGraph(double[,] g)
        {
            for (int i = 0; i < g.GetLength(0); i++)
            {
                for (int j = 0; j < g.GetLength(1); j++)
                {
                    if (g[i, j] == double.MaxValue)
                    {
                        Console.Write("INF ");
                    }
                    else if(g[i, j] == 0)
                    {
                        Console.Write("0.000 ");
                    }
                    else
                    {
                        Console.Write(String.Format("{0:0.000}", g[i, j]) + " ");
                    }
                }
                Console.Write(System.Environment.NewLine);
            }
        }
    }
}
