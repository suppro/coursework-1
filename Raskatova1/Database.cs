using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Raskatova1
{
    class Database
    {
        private SQLiteConnection CONN;
        private string FileName;

        public Database(string FileName)
        {
            this.FileName = FileName;
        }

        public void OpenConnection()
        {
            CONN = new SQLiteConnection("Data Source=" + FileName + "; " + "Version=3");
            CONN.Open();
        }

        public void CloseConnection()
        {
            CONN.Close();
        }

        public void LoadResults(List<Result> listResult)
        {
            SQLiteCommand CMD = new SQLiteCommand("insert into Results(Name, Age, TimeOfStart, TimeOfFinish, TimeOfTotal) values(@name, @age, @timeofstart, @timeoffinish, @timeoftotal)", CONN);
            for (int i = 0; i < listResult.Count; i++)
            {
                CMD.Parameters.Add("@name", System.Data.DbType.String).Value = listResult[i].FullName;
                CMD.Parameters.Add("@age", System.Data.DbType.String).Value = listResult[i].Age;
                CMD.Parameters.Add("@timeofstart", System.Data.DbType.Int32).Value = listResult[i].TimeOfStart;
                CMD.Parameters.Add("@timeoffinish", System.Data.DbType.Int32).Value = listResult[i].TimeOfFinish;
                CMD.Parameters.Add("@timeoftotal", System.Data.DbType.Int32).Value = listResult[i].TotalTime;
                CMD.ExecuteNonQuery();
            }
            CMD.Dispose();
        }

        public void PrintTable()
        {
            TimeSpan tsTotal, tsFinish, tsStart;
            SQLiteCommand CMD = new SQLiteCommand("select * from Results", CONN);
            SQLiteDataReader RD = CMD.ExecuteReader();

            Console.WriteLine("Имя бегуна\t\t" + "Возраст\t" + "Время старта\t" + "Время финиша\t" + "Результат");
            if (RD.HasRows)
            {
                while (RD.Read())
                {
                    tsTotal = TimeSpan.FromSeconds(Convert.ToInt32(RD["TimeOfTotal"]));
                    tsStart = TimeSpan.FromSeconds(Convert.ToInt32(RD["TimeOfStart"]));
                    tsFinish = TimeSpan.FromSeconds(Convert.ToInt32(RD["TimeOfFinish"]));
                    Console.WriteLine($"{RD["Name"],-24}" + RD["Age"]
                        + "\t" + "{0}:{1}:{2}" + "\t\t" + "{3}:{4}:{5}" + "\t\t" + "{6}:{7}:{8}",
                        tsStart.Hours, tsStart.Minutes, tsStart.Seconds,
                        tsFinish.Hours, tsFinish.Minutes, tsFinish.Seconds,
                        tsTotal.Hours, tsTotal.Minutes, tsTotal.Seconds);
                }

                RD.Close();
                CMD.Dispose();
            }
        }

        public string GetFileName() { return FileName; }
    }
}
