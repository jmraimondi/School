using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vAppDevProj1
{
    class Program
    {
        static void Main(string[] args)
        {
            Help();
            ConsoleKeyInfo k = Console.ReadKey(true);
            while (k.Key != ConsoleKey.Escape) { 
                if (k.Key == ConsoleKey.D1)
                {
                    Triangles();
                }
                else if (k.Key == ConsoleKey.D2)
                {
                    Triples();
                }
                else if (k.Key == ConsoleKey.D3)
                {
                    Palindromes();
                }
                Console.WriteLine();
                Help();
                k = Console.ReadKey(true);
            }
        }

        static void Help()
        {
            Console.WriteLine("1. Triangles");
            Console.WriteLine("2. Triples");
            Console.WriteLine("3. Palindromes");
            Console.WriteLine();
        }
        static void Triangles()
        {
            Console.WriteLine("Triangles");
            //9
            for(int i = 0; i < 9; i++)
            {
                Print('*', i + 1);
                Print(' ', 9 - i);
                Print('*', 9 - i);
                Print(' ', (2*i+1));
                Print('*', 9 - i);
                Print(' ', 9 - i);
                Print('*', i + 1);
                Console.WriteLine();
            }
        }

        static void Print(char c, int n)
        {
            for (int i = 0; i < n; i++)
            {
                Console.Write(c);
            }
        }

        static void Triples()
        {
            for (double i = 1; i < 500; i++)
            {
                for(double j = 1; j < 500; j++)
                {
                    if((Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2))) % 1 == 0 && (Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2)) < 500))
                    {
                        string tmp = "(" + i + "," + j + "," + Math.Sqrt(Math.Pow(i, 2) + Math.Pow(j, 2)) + ")";
                        Console.WriteLine(tmp);
                    }
                }
            }
        }

        static void Palindromes()
        {
            Console.WriteLine("Palindromes");
            Console.WriteLine();
            Console.Write("Enter a 5 digit number: ");
            string num = Console.ReadLine();

            while(num.Length != 5 || !(num.All(char.IsDigit)))
            {
                Console.Write("Please enter a number with length 5: ");
                num = Console.ReadLine();
            }

            if((int)num[0] == (int)num[4] && (int)num[1] == (int)num[3])
            {
                Console.WriteLine("Yes");
            }
            else
            {
                Console.WriteLine("No");
            }
        }
    }
}
