using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConvexHull
{
    class Hull
    {
        static void Main(string[] args)
        {
            GenerateFile();
            print(FindHull(ReadPoints()));

            Console.ReadKey();
        }

        public static void GenerateFile()
        {
            StreamWriter sw = new StreamWriter(@"C:\Users\JM\projects\algAnalysis\ConvexHull\ConvexHull\points.txt");
            Random rand = new Random();
            for(int i = 0; i < rand.Next(50, 201); i++)
            {
                sw.WriteLine("{0},{1}", rand.Next(1001), rand.Next(1001));
            }
            sw.Close();
        }
        
        public static List<Point> ReadPoints()
        {
            List<Point> pts = new List<Point>();
            StreamReader sr = new StreamReader(@"C:\Users\JM\projects\algAnalysis\ConvexHull\ConvexHull\points.txt");
            try
            {
                String line = sr.ReadLine();
                while (line != null)
                {
                    string[] pt = line.Split(',');
                    pts.Add(new Point(Convert.ToInt32(pt[0]), Convert.ToInt32(pt[1])));
                    line = sr.ReadLine();
                }
            }
            catch { }
            sr.Close();

            return pts;
        }
        
        public static List<Point> FindHull(List<Point> pts)
        {
            List<Point> hull = new List<Point>();
            for(int i = 0; i < pts.Count; i++)
            {
                for(int j = 0; j < pts.Count; j++)
                {
                    if(i == j)
                    {
                        continue;
                    }
                    bool? gt = null;
                    bool onHull = true;

                    for (int k = 0; k < pts.Count; k++)
                    {
                        if (k == i || k == j)
                        {
                            continue;
                        }
                        int val = (pts[k].X - pts[i].X) * (pts[j].Y - pts[i].Y) - (pts[k].Y - pts[i].Y) * (pts[j].X - pts[i].X);

                        if (gt == null)
                        {
                            if(val > 0)
                            {
                                gt = true;
                            }
                            else if(val < 0)
                            {
                                gt = false;
                            }
                        }

                        if (gt == true && val < 0)
                        {
                            onHull = false;
                            break;
                        }
                        else if(gt == false && val > 0)
                        {
                            onHull = false;
                            break;
                        }
                    }
                    if(onHull == true)
                    {
                        if (!hull.Contains(pts[i]))
                        {
                            hull.Add(pts[i]);
                        }
                        if (!hull.Contains(pts[j]))
                        {
                            hull.Add(pts[j]);
                        }
                    }
                }
                
            }

            return hull;
        }

        public static void print(List<Point> pts)
        {
            StreamWriter sw = new StreamWriter(@"C:\Users\JM\projects\algAnalysis\ConvexHull\ConvexHull\hullPoints.txt");
            foreach (var pt in pts)
            {
                string str = "(" + pt.X.ToString() + "," + pt.Y.ToString() + ")";
                Console.WriteLine(str);
                sw.WriteLine(str);
            }
            sw.Close();
        }

        public struct Point
        {
            public int X;
            public int Y;

            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
    }
}
