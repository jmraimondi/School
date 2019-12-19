using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace bitString
{
    class Bit
    {
        static void Main(string[] args)
        {
            uint bp = 0b10001;
            uint bs = 0b11011000110110111;
            uint bp2 = 0b111;
            uint bp3 = 01100;

            print(bp, bs, find(bp, bs));
            print(bp2, bs, find(bp2, bs));
            print(bp3, bs, find(bp3, bs));
        }

        public static int find(UInt32 bitPattern, UInt32 bitString)
      {
            int bplength = Convert.ToString(bitPattern, 2).Length;//ToString().Length;
            //int bslength = bitString.ToString().Length;

            uint str = bitString;
            uint patt = bitPattern;
            string test = Convert.ToString(bitString, 2);
            for (int bslength = Convert.ToString(bitString, 2).Length; bslength - bplength > -1; bslength--)
            {
                str &= (uint)~(1 << bslength);
                uint tmp = (str >> bslength - bplength);
                if((patt ^ tmp) == 0)
                {
                    uint ret = (str << 32 - bslength);
                    return (int)ret; //left shifted int
                }
                
            }
            return 0;
      }

    public static void print(UInt32 bp, UInt32 bs, int result)
        {
            //StreamWriter sw = new StreamWriter(@"C:\Users\JM\projects\algAnalysis\bitString\bitString\out.txt");
            StreamWriter sw = File.AppendText(@"C:\Users\JM\projects\algAnalysis\bitString\bitString\out.txt");
            Console.WriteLine("Pattern: {0}", Convert.ToString(bp,2));
            sw.WriteLine("Pattern: {0}", Convert.ToString(bp, 2));

            Console.WriteLine("Search string: {0}", Convert.ToString(bs,2));
            sw.WriteLine("Search string: {0}", Convert.ToString(bs, 2));

            Console.WriteLine("Result: {0}", Convert.ToString(result, 2));
            sw.WriteLine("Result: {0}", Convert.ToString(result, 2));
            sw.WriteLine("-------------------------------------------------------------");
            sw.Close();
            //Console.ReadKey();

        }
    }
}
