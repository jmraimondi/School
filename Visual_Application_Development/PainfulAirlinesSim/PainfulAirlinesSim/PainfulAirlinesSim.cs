using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PainfulAirlinesSim
{
    
    enum ePriority { Economy, Business, FirstClass }
    class PainfulAirlinesSim
    {
        static void Main(string[] args)
        {
            Line EconomyLine = new Line(0);
            Line BusinessLine = new Line(1);
            Line FirstClassLine = new Line(2);
             
            for(int currentTime = 1; currentTime < 722; currentTime++) //finish 720th minute...?
            {
                if (currentTime % 3 == 0)
                    EconomyLine.SpawnCustomer(currentTime);
                if (currentTime % 10 == 0)
                {
                    Console.WriteLine("Current time: {0}", currentTime);
                    FirstClassLine.Print();
                    BusinessLine.Print();
                    EconomyLine.Print();
                    Console.WriteLine();
                }
                if (currentTime % 15 == 0)
                    BusinessLine.SpawnCustomer(currentTime);
                if (currentTime % 30 == 0)
                    FirstClassLine.SpawnCustomer(currentTime);
                EconomyLine.AssistCustomer(FirstClassLine, BusinessLine, currentTime);
                BusinessLine.AssistCustomer(FirstClassLine, EconomyLine, currentTime);
                FirstClassLine.AssistCustomer(BusinessLine, EconomyLine, currentTime);

                EconomyLine.UpdateStats();
                BusinessLine.UpdateStats();
                FirstClassLine.UpdateStats();

            }
            FirstClassLine.PrintFinal();
            BusinessLine.PrintFinal();
            EconomyLine.PrintFinal();
        }
    }

    class Line
    {
        public readonly int Priority;
        private Queue<Customer> inLine;
        public Customer currentCustomer;
        private List<int> TimeSpentInLine;
        private List<int> LineLength;
        private int MaxLineLength = 0;
        private int CustomersAssisted = 0;

        private static Random rnd;

        public Line(int p)
        {
            Priority = p;
            currentCustomer = null;
            inLine = new Queue<Customer>();
            TimeSpentInLine = new List<int>();
            LineLength = new List<int>();
            rnd = new Random();
        }
        public bool HasCustomers()
        {
            if(inLine.Count != 0)
            {
                return true;
            }
            return false;
        }
        public void SpawnCustomer(int time)
        {
            inLine.Enqueue(new Customer(Priority, time, rnd, this));
        }
        public void AssistCustomer(Line l1, Line l2, int time)
        {
            if (currentCustomer == null)
            {
                if (HasCustomers())
                {
                    currentCustomer = inLine.Dequeue();
                }
                else
                {
                    if(l1.HasCustomers())
                    {
                        currentCustomer = l1.inLine.Dequeue();
                    }
                    else if(l2.HasCustomers())
                    {
                        currentCustomer = l2.inLine.Dequeue();
                    }
                }
            }
            if (currentCustomer != null)
            {
                currentCustomer.TimeToHelp -= 1;
                if(currentCustomer.TimeToHelp == 0)
                {
                    currentCustomer.OriginalLine.TimeSpentInLine.Add(time - currentCustomer.StartTime + 1);
                    currentCustomer = null;
                    CustomersAssisted++;
                }
                return;
            }
            

        }
        public void Print()
        {
            if (currentCustomer != null)
                Console.WriteLine("{0}: Agent assisting customer. {1} customers currently in line.", Enum.GetName(typeof(ePriority), Priority), inLine.Count);
            else
                Console.WriteLine("{0}: Agent not assisting customer. {1} customers currently in line.", Enum.GetName(typeof(ePriority), Priority), inLine.Count);
        }
        public void UpdateStats()
        {
            if (inLine.Count > MaxLineLength)
                MaxLineLength = inLine.Count;
            LineLength.Add(inLine.Count);

        }
        public void PrintFinal()
        {
            Console.WriteLine("{0}:\nAverage time spent in line: {1}\nTotal customers assisted: {2}\nAverage line length: {3}\nMax line length: {4}\n",
                Enum.GetName(typeof(ePriority), Priority), TimeSpentInLine.Average(), CustomersAssisted, LineLength.Average(), MaxLineLength);
        }
    }

    class Customer
    {
        public int Priority;
        public int TimeToHelp;
        public readonly int StartTime;
        public readonly Line OriginalLine;

        public Customer(int p, int st, Random rnd, Line l)
        {
            Priority = p;
            StartTime = st;
            TimeToHelp = getTTH(Priority, rnd);
            OriginalLine = l;
        }

        private static int getTTH(int p, Random rnd) //time to help
        {
            switch ((ePriority)p)
            {
                case ePriority.FirstClass:
                    return rnd.Next(5, 21); //5 to 20 min
                case ePriority.Business:
                    return rnd.Next(6, 13); //6 to 12
                case ePriority.Economy:
                default:
                    return rnd.Next(5, 11); //5 to 10
            }
        }
    }
}
