using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dataObservations
{
    class DataObservations
    {

        private Dictionary<string, List<int>> ObservationDictionary;
        
        public Dictionary<string, List<int>> getDictionary()
        {
            return ObservationDictionary;
        }
        public void setDictionary(string obname, List<int> obsList)
        {
            if (ObservationDictionary.ContainsKey(obname))
            {
                return;
            }
            else
            {
                ObservationDictionary.Add(obname, obsList);
            }
        }
        public void LoadFromFile(string fname)
        {
            string[] lines = System.IO.File.ReadAllLines(fname);

            foreach (string line in lines)
            {
                string[] tmpStr = line.Split();
                List<int> tmpInts = null;
                for(int i = 1; i < tmpStr.Length; ++i)
                {
                    tmpInts.Add(Convert.ToInt32(tmpStr[i]));
                }
                setDictionary(tmpStr[0], tmpInts);
            }
        }
        public void SaveToFile(string fname)
        {
            StringBuilder txt = new StringBuilder();

            foreach(KeyValuePair<string, List<int>> e in ObservationDictionary)
            {
                txt.Append(e.Key);
                txt.Append(" ");
                foreach(int o in e.Value)
                {
                    txt.Append(o);
                    txt.Append(" ");
                }
                txt.Remove(txt.Length, 1);
                txt.AppendLine();

            }
            System.IO.File.WriteAllText(fname, txt.ToString());
        }
    }
}
