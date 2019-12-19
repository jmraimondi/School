using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace csharpdu
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please enter path of folder/file");
                return;
            }

            FileStream filestream = new FileStream("out.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);

            string startPath = args[0];
            IEnumerable<string> fList = System.IO.Directory.GetFiles(startPath, "*.*", System.IO.SearchOption.AllDirectories);
            var files = from f in fList select GetFileLength(f);
            long[] fLengths = files.ToArray();
            Console.WriteLine(files.Sum());
            Console.ReadKey();
        }

        static long GetFileLength(string fname)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(fname);
            long length = fi.Length;
            Console.WriteLine("{0} {1}", length, fname);
            return length;
        }
    }
}
