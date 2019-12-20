using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient; //https://dev.mysql.com/downloads/connector/net/

//Reads each record in DB creating SQL queries and splitting them into appropriate files.

namespace sqlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string connection = "Data Source=localhost;Initial Catalog=veteransmuseum;User ID=root;Password=";
            string selectQuery = "SELECT * FROM vmerge";
            MySqlConnection dbcon = new MySqlConnection(connection);
            MySqlCommand cmd = new MySqlCommand(selectQuery, dbcon);
            MySqlDataReader reader;
            try
            {
                dbcon.Open();
                reader = cmd.ExecuteReader(); 
                while (reader.Read())
                {
                    int ID = reader.GetInt32("ID");

                    string branch1 = "UNKNOWN";//0
                    string branch2 = "UNKNOWN";//1
                    string rank1 = "UNKNOWN";//2
                    string rank2 = "UNKNOWN";//3
                    string ship = "";//4
                    string awards = "";//5
                    string c1 = ""; 
                    string c2 = "";
                    string c3 = "";
                    string s1 = "INSERT INTO Services (ID,Branch,SRank";
                    string s2 = " VALUES (";
                    string csql = "INSERT INTO Conflicts (ID, ConflictName) VALUES (";
                    string cmd1 = s1;
                    string cmd2 = s1;
                    string comments = "";

                    List<String> cmds = new List<string>();
                    List<String> acmds = new List<string>();
                    List<String> ccmds = new List<string>();

                    bool[] sData = new bool[6];
                    if(!Convert.IsDBNull(reader["Branch1"])) //have b1
                    {
                        sData[0] = true;
                        branch1 = reader.GetString("Branch1");
                        if (!Convert.IsDBNull(reader["Awards"])) //have b1 AND award
                        {
                            sData[5] = true;
                            awards = reader.GetString("Awards");
                        }

                    }   
                    if (!Convert.IsDBNull(reader["Rank1"])) //have r1
                    {
                        sData[1] = true;
                        rank1 = reader.GetString("Rank1"); 
                    }
                    if(!Convert.IsDBNull(reader["Branch2"])) //have b2
                    {
                        sData[2] = true;
                        branch2 = reader.GetString("Branch2");  
                    }
                    if (!Convert.IsDBNull(reader["Rank2"])) //have r2
                    { 
                        sData[3] = true;
                        rank2 = reader.GetString("rank2");
                    
                    }
                    if (!Convert.IsDBNull(reader["UnitShip"])) //have ship
                    {
                        sData[4] = true;
                        ship = reader.GetString("UnitShip");
                    }
                    
                    if(sData[4])
                    {
                        cmd1 += ",UnitShip)" + s2 + ID.ToString() + ",'" + branch1 + "','" + rank1 + "','" + ship + "');"; //find: ([a-zA-Z])'S replace: $1''S
                    }
                    else
                    {
                        cmd1 += ")" + s2 + ID.ToString() + ",'" + branch1 + "','" + rank1+ "');";
                    }
                    cmds.Add(cmd1);
                    if(sData[2] || sData[3]) //have b2 or r2
                    {
                        cmd2 += ")" + s2 + ID.ToString() + ",'" + branch2 + "','" + rank2 + "');";
                        cmds.Add(cmd2);
                    }
                    //write cmds
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\JM\Desktop\addServices.txt", true)) //append
                    {
                        foreach (string c in cmds)
                        {
                            file.WriteLine(c);
                        }
                    }

                    if(sData[5]) //have branch and award(s)
                    {
                        if (awards == "alot")
                        {
                            awards = "NATIONAL DEFENSE SERVICE MEDAL%Navy Good Conduct Medal%Navy Meritorious Unit Commendation%SILVER DOLPHIN%FLEET BALISTIC MISSILE WITH TWO STARS";
                        }
                        string[] acmd = awards.Split('%');
                        string iString = "INSERT INTO Awards (ID, AwardName, BranchName) VALUES (" + ID.ToString() + ",'";
                        for (int i = 0; i < acmd.Length; i++)
                        {
                            acmd[i] = acmd[i].Insert(0, iString);
                            acmd[i] += "','" + branch1 + "');";
                            acmds.Add(acmd[i]);
                        }
                        //write award cmds
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\JM\Desktop\addAwards.txt", true)) //append
                        {
                            foreach (string c in acmds)
                            {
                                file.WriteLine(c);
                            }
                        }
                    }

                    if (!Convert.IsDBNull(reader["conflict1"])) //have c1
                    {
                        c1 = reader.GetString("conflict1");
                        ccmds.Add(csql + ID.ToString() + ",'" + c1 + "');");

                        if (!Convert.IsDBNull(reader["conflict2"])) //have c2
                        {
                            c2 = reader.GetString("conflict2");
                            ccmds.Add(csql + ID.ToString() + ",'" + c2 + "');");

                            if (!Convert.IsDBNull(reader["conflict3"])) //have c3
                            {
                                c3 = reader.GetString("conflict3");
                                ccmds.Add(csql + ID.ToString() + ",'" + c3 + "');");

                            }

                        }

                    }

                    if(ccmds.Count != 0)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\JM\Desktop\addConflicts.txt", true)) //append
                        {
                            foreach (string c in ccmds)
                            {
                                file.WriteLine(c);
                            }
                        }
                    }

                    if (!Convert.IsDBNull(reader["Comments"])) //have comments
                    {

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\JM\Desktop\addComments.txt", true)) //append
                        {
                            comments = reader.GetString("Comments");
                            file.WriteLine("INSERT INTO UserComments (ID, UserComment) VALUES (" + ID.ToString() + ",'" + comments + "');" );
                        }
                    }

                }
                reader.Close();
                dbcon.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            
        }
    }
}
