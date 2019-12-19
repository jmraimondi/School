using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sudoku
{
    static class SudokuSolver
    {
        static Board ReadPuzzle()
        {
            StreamReader sr = new StreamReader("infile.txt");
            string line;
            Board board = new Board();
            for(int i = 0; i < 9; i++)
            {
                line = sr.ReadLine();
                string[] split = line.Split(',');
                for(int j = 0; j < 9; j++)
                {
                    board[i, j] = Convert.ToInt32(split[j]);
                }
            }

            sr.Close();
            return board;
        }

        public static bool solve(ref Board b)
        {
            int x = -1;
            int y = -1;
            bool solved = true;

            for(int i = 0; i < 9 && solved; i++)
            {
                for(int j = 0; j < 9; j++)
                {
                    if(b[i,j] == 0)
                    {
                        solved = false;
                        x = i;
                        y = j;
                        break;
                    }
                }
            }

            if(solved)
            {
                return true;
            }

            for(int v = 1; v < 10; v++)
            {
                if(b.validMove(x,y,v))
                {
                    b[x, y] = v;
                    if(solve(ref b))
                    {
                        return true;
                    }
                    else
                    {
                        b[x, y] = 0;
                    }
                }
            }
            return false;
        }

        public class Board
        {
            private int[,] gameBoard;

            public Board()
            {
                gameBoard = new int[9, 9];
            }

            public int this[int x, int y]
            {

                get
                {
                    if (x > -1 && x < 9 && y > -1 && y < 9)
                    {
                        return gameBoard[x, y];
                    }
                    else
                    {
                        return -1;
                    }
                }
                set
                {
                    if (x > -1 && x < 9 && y > -1 && y < 9)
                    {
                        if(value > -1 && value < 10)
                        {
                            gameBoard[x, y] = value;
                        }
                        
                    }
                    else
                    {
                        return;
                    }
                }
            }

            private bool validGrid(int x, int y, int v)
            {
                if(gameBoard[x,y] != 0)
                {
                    return false;
                }
                grid tmp = getGrid(x, y);
                for(int i = tmp.x; i < tmp.x+3; i++)
                {
                    for(int j = tmp.y; j < tmp.y+3; j++)
                    {
                        if(gameBoard[i,j] == v)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public bool validMove(int x, int y, int v)
            {
                if(validCol(x, y, v) && validRow(x, y, v) && validGrid(x, y, v))
                {
                    return true;
                }
                return false;
            }

            private bool validRow(int x, int y, int v)
            {
                if(gameBoard[x,y] != 0)
                {
                    return false;
                }
                for(int i = 0; i < 9; i ++)
                {
                    if(gameBoard[x, i] == v)
                    {
                        return false;
                    }
                }
                return true;
            }

            private bool validCol(int x, int y, int v)
            {
                if (gameBoard[x, y] != 0)
                {
                    return false;
                }
                for (int i = 0; i < 9; i++)
                {
                    if (gameBoard[i, y] == v)
                    {
                        return false;
                    }
                }
                return true;
            }

            private grid getGrid(int x, int y)
            {
                grid g = new grid();
                switch(x)
                {
                    case 0:
                    case 1:
                    case 2:
                        g.x = 0;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        g.x = 3;
                        break;
                    case 6:
                    case 7:
                    case 8:
                        g.x = 6;
                        break;
                    default:
                        break;
                }
                switch(y)
                {
                    case 0:
                    case 1:
                    case 2:
                        g.y = 0;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        g.y = 3;
                        break;
                    case 6:
                    case 7:
                    case 8:
                        g.y = 6;
                        break;
                    default:
                        break;
                }

                return g;
            }

            private struct grid
            {
                public int x;
                public int y;
            }

            public void print()
            {
                char c1 = '\u250C';
                char c2 = '\u2510';
                char c3 = '\u2514';
                char c4 = '\u2518';
                char T1 = '\u252C';
                char T2 = '\u251C';
                char T3 = '\u2524';
                char T4 = '\u2534';
                char v = '\u2502';  
                char x = '\u253C';

                for (int i = 0; i < 9; i++)
                {
                    if(i == 0)
                    {
                        Console.WriteLine(getLine(c1, T1, c2));
                    }
                    else
                    {
                        Console.WriteLine(getLine(T2, x, T3));
                    }
                    
                    for(int j = 0; j < 9; j++)
                    {
                        Console.Write(v);
                        Console.Write(gameBoard[i, j]);
                        if(j == 8)
                        {
                            Console.WriteLine(v);
                        }
                    }
                }
                Console.WriteLine(getLine(c3, T4, c4));
            }

            private string getLine(char start, char m, char end)
            {
                string str = "";
                char h = '\u2500'; //might be middle bar
                char[] c = new char[] { start, h, m, h, m, h, m, h, m, h, m, h, m, h, m, h, m, h, end };
                foreach(char ch in c)
                {
                    str += ch.ToString();
                }
                return str;
            }
        }


        static void Main(string[] args)
        {
            Board b = ReadPuzzle();
            b.print();
            Console.WriteLine();
            Console.WriteLine();
            solve(ref b);
            b.print();
            Console.ReadKey();
        }
    }
}
