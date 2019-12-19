using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CoinsInaRow
{
    class Coins
    {
        static bool solve(ref CoinRow c) // ends must be 121 - 121 OR 2 - 2 |||||| 8 minimum + 4's
        { 
            if(c.startingCoins() % 2 != 0)
            {
                return false;
            }

            if(!c.movesLeft())
            {
                if(c.completed())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            for(int i = 0; i < c.count() - 1; i++)
            {
                if(c.canJumpRight(i) != -1)
                {
                    c.doJumpRight(i);
                    if(solve(ref c))
                    {
                        return true;
                    }
                    else
                    {
                        c.undo();
                    }
                }
                if(c.canJumpLeft(i) != -1)
                {
                    c.doJumpLeft(i);
                    if(solve(ref c))
                    {
                        return true;
                    }
                    else
                    {
                        c.undo();
                    }
                }
            }
            return false;
        }

        static void Main(string[] args)
        {
            List<int> solutions = new List<int>();

            for (int i = 1; i < 21; i++)
            {
                CoinRow c = new CoinRow(i);
                if(solve(ref c))
                {
                    solutions.Add(i);
                }
            }
            Console.WriteLine("Number of coins with solutions");
            foreach(int i in solutions)
            {
                Console.Write("{0} ", i);
            }
            Console.ReadKey();
        }



        public class CoinRow
        {
            private List<int> coins;
            private int moves;
            private readonly List<int> m1 = new List<int> { 1, 1, 1, 1 };
            private readonly List<int> m2 = new List<int> { 1, 2, 1 };
            private Stack<Move> moveList;
            private int numCoins;

            public CoinRow(int n)
            {
                moves = 0;
                coins = new List<int>();
                moveList = new Stack<Move>();
                numCoins = n;

                for(int i = 0; i < n; i++)
                {
                    coins.Add(1);
                }
            }

            public int count()
            {
                return coins.Count;
            }

            public int startingCoins()
            {
                return numCoins;
            }

            public void undo()
            {
                if(moveList.Count > 0)
                {
                    Move tmp = moveList.Pop();
                    coins.Insert(tmp.indexFrom, 1);
                    coins[tmp.indexTo] = 1;
                }
            }

            public bool movesLeft()
            {
                for(int i = 0; i < coins.Count - (m1.Count - 1); i++)
                {
                    bool match = true;
                    for(int j = 0; j < m1.Count; j++)
                    {
                        if(coins[i + j] != m1[j])
                        {
                            match = false;
                            break;
                        } 
                    }
                    if(match)
                    {
                        return true;
                    }
                }

                for (int i = 0; i < coins.Count - (m2.Count - 1); i++)
                {
                    bool match = true;
                    for (int j = 0; j < m2.Count; j++)
                    {
                        if (coins[i + j] != m2[j])
                        {
                            match = false;
                            break;
                        }
                    }
                    if(match)
                    {
                        return true;
                    }
                }
                return false;
            }

            public int this[int i]
            {
                get
                {
                    if (i < 0 || i > coins.Count - 1)
                    {
                        return -1;
                    }
                    return coins[i];
                }

                private set
                {
                    coins[i] = value;
                }
            }

            public int canJumpRight(int i)
            {
                if(i > -1 && i < coins.Count - 2 && coins[i] == 1)
                {
                    if(i == coins.Count - 3)
                    {
                        if(coins[i+1] == 2 && coins[i+2] == 1)
                        {
                            return i+2;
                        }
                        return -1;
                    }
                    else
                    {
                        if(coins[i+1] == 2 && coins[i+2] == 1)
                        {
                            return i+2;
                        }
                        else if(coins[i+1] == 1 && coins[i+2] == 1 && coins[i+3] == 1)
                        {
                            return i+3;
                        }
                        return -1;
                    }
                }
                return -1;
            }

            public int canJumpLeft(int i)
            {
                if (i > 1 && i < coins.Count && coins[i] == 1)
                {
                    if (i == 2)
                    {
                        if (coins[1] == 2 && coins[0] == 1)
                        {
                            return 0;
                        }
                        return -1;
                    }
                    else
                    {
                        if (coins[i - 1] == 2 && coins[i - 2] == 1)
                        {
                            return i-2;
                        }
                        else if (coins[i - 1] == 1 && coins[i - 2] == 1 && coins[i - 3] == 1)
                        {
                            return i-3;
                        }
                        return -1;
                    }
                }
                return -1;
            }

            public void doJumpRight(int i, bool d=false)
            {
                int jump = canJumpRight(i);
                if(jump != -1)
                {
                    moveList.Push(new Move(i, jump));
                    coins[jump] = 2;
                    coins.RemoveAt(i);
                    moves++;
                    if(d)
                    {
                        Display();
                    }
                }
            }

            public void doJumpLeft(int i, bool d = false)
            {
                int jump = canJumpLeft(i);
                if (jump != -1)
                {
                    moveList.Push(new Move(i, jump));
                    coins[jump] = 2;
                    coins.RemoveAt(i);
                    moves++;
                    if (d)
                    {
                        this.Display();
                    }
                }
            }

            public bool completed()
            {
                for(int i = 0; i < coins.Count; i++)
                {
                    if(coins[i] == 1)
                    {
                        return false;
                    }
                }
                return true;
            }

            public void Display()
            {
                for(int i = 0; i < coins.Count; i++)
                {
                    Console.Write("({0}) ", coins[i]);
                }
                Console.WriteLine();
                Console.WriteLine("moves: {0}", moves);
            }

            private struct Move
            {
               public readonly int indexFrom;
               public readonly int indexTo;

                public Move(int f, int t)
                {
                    this.indexFrom = f;
                    this.indexTo = t;
                }
            }
        }
    }
}
