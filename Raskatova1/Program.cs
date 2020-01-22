//ДОДЕЛАЙ ПРОГУ
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data.SQLite;

namespace Raskatova1
{
    class Program
    {
        private static Database DB;

        public static string FullName(List<string> namesStorage)
        {
            Random rnd = new Random();
            int skip = rnd.Next(0, namesStorage.Count());
            string line = namesStorage.ElementAt(skip);
            namesStorage.RemoveAt(skip);
            return line;
        }

        public static List<Result> NewResult(int numberOfRunners, string fileName)
        {
            int age, start, finish, total;
            string rank;

            Random rnd = new Random();
            List<string> file = File.ReadLines(fileName).ToList();
            List<Result> listResult = new List<Result>();
            
            for (int i = 0; i < numberOfRunners; ++i)
            {
                age = rnd.Next(18, 80);
                rank = AtleteRank(age);
                start = rnd.Next(0, 180);
                total = FinishTime(rank, age, rnd);
                finish = total + start;
                listResult.Add(new Result() { FullName = FullName(file), Age = age, Rank = rank, TimeOfStart = start, TimeOfFinish = finish, TotalTime = total });
            }
            listResult.Sort((Result x, Result y) => { return x.TotalTime.CompareTo(y.TotalTime); });
            return listResult;
        }

        public static void AgeResult(List<Result> listResult, int ageMin, int ageMax)
        {
            int timeWiner = -1;
            int ageWinner = 0;
            string nameWiner = "";
            bool flag = false;

            for (int i = 0; i < listResult.Count; i++)
            {
                if (listResult[i].Age >= ageMin && listResult[i].Age <= ageMax && ((listResult[i].TimeOfFinish - listResult[i].TimeOfStart) < timeWiner || timeWiner == -1))
                {
                    timeWiner = listResult[i].TotalTime;
                    nameWiner = listResult[i].FullName;
                    ageWinner = listResult[i].Age;
                    flag = true;
                }
            }

            TimeSpan ts = TimeSpan.FromSeconds(timeWiner);
            if(flag) Console.WriteLine("В возрастной группе " + ageMin + " - " + ageMax + " лет побеждает " + nameWiner + " (" + ageWinner + " лет) с результатом " + "{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
            else Console.WriteLine("В возрастной группе " + ageMin + " - " + ageMax + " лет нет атлетов");
        }

        public static int FinishTime(string rank, int age, Random rnd)
        {
            int finish;
            int addTime = age * 2;
            switch(rank)
            {
                case "Пожилой":
                    finish = rnd.Next(5000 + addTime, 8000 + addTime); break;
                case "Начинающий":
                    finish = rnd.Next(3601, 7600); break;
                case "Любитель":
                    finish = rnd.Next(2341, 3301); break;
                case "Атлет":
                    finish = rnd.Next(1951, 2340); break;
                case "Элита":
                    finish = rnd.Next(1740, 1950); break;
                default:
                    throw new Exception("Ранг не определен");
            }
            return finish;
        }

        public static string AtleteRank(int age)
        {
            string rank;
            double addChance = 0.03 + (age - 45) * 0.03;

            Random rnd = new Random();
            double setRank = rnd.NextDouble();
            if (age >= 60) { rank = "Пожилой"; }
            else
            {
                if (age >= 45) setRank += addChance;

                if (setRank <= 0.02) { rank = "Элита"; }
                else if (setRank > 0.02 && setRank <= 0.15) { rank = "Атлет"; }
                else if (setRank > 0.15 && setRank <= 0.5) { rank = "Любитель"; }
                else if (setRank > 0.5 && setRank <= 2) { rank = "Начинающий"; }
                else { throw new Exception("Ранг не определен"); }
            }
            return rank;
        }
        public static void SortResult(List<Result> listResult, int ageMin, int ageMax, bool subMenu)
        {
            TimeSpan tsTotal, tsFinish, tsStart;
            var allResultInAge = from age in listResult
                                 where age.Age >= ageMin && age.Age <= ageMax
                                 orderby age.TotalTime ascending
                                 select age;
            Console.WriteLine("Имя бегуна\t\t" + "Возраст\t" + "Время старта\t" + "Время финиша\t" + "Результат");
            foreach (Result age in allResultInAge)
            {
                tsTotal = TimeSpan.FromSeconds(age.TotalTime);
                tsStart = TimeSpan.FromSeconds(age.TimeOfStart);
                tsFinish = TimeSpan.FromSeconds(age.TimeOfFinish);
                Console.WriteLine($"{age.FullName,-24}" + age.Age
                    + "\t" + "{0}:{1}:{2}" + "\t\t" + "{3}:{4}:{5}" + "\t\t" + "{6}:{7}:{8}",
                    tsStart.Hours, tsStart.Minutes, tsStart.Seconds,
                    tsFinish.Hours, tsFinish.Minutes, tsFinish.Seconds,
                    tsTotal.Hours, tsTotal.Minutes, tsTotal.Seconds);
            }

            CheckSubMenu(listResult, subMenu);
        }

        public static void CheckSubMenu(List<Result> listResult, bool subMenu)
        {
            string answer;
            if (subMenu)
            {
                Console.WriteLine("Чтобы вернуться в подменю введите 1, чтобы вернуться на главное меню нажмите 2");
                answer = Console.ReadLine();

                switch(answer)
                {
                    case "1":
                        Menu(listResult, true); break;
                    case "2":
                        Menu(listResult, false); break;
                    default:
                        Console.WriteLine("Ошибка ввода: данного варианта ответа не существует. Возврат в подменю...");
                        Console.ReadKey();
                        Menu(listResult, true);
                        break;
                }
            }
            else
            {
                Console.WriteLine("Для возврата на главную нажмите на любую клавишу.");
                Console.ReadKey();
                Menu(listResult, false);
            }
        }
        public static void Menu(List<Result> listResult, bool subMenu)
        {
            string answer;
            while (true)
            {
                if (!subMenu)
                {
                    Console.Clear();
                    Console.WriteLine("Забег на 10 км.");
                    Console.WriteLine("Всего участников забега - " + listResult.Count);
                    Console.WriteLine("Для вывода результатов в возрастных группах введите 1");
                    Console.WriteLine("Для вывода результатов забега введите 2");
                    Console.WriteLine("Для вывода результатов за всю историю соревнований 3");
                    Console.WriteLine("Для выхода из программы введите 0");
                    answer = Console.ReadLine();
                }
                else answer = "1";

                switch(answer)
                {
                    case "0":
                        throw new Exception("Выход из меню");
                    case "1":
                        Console.Clear();
                        AgeResult(listResult, 18, 30);
                        AgeResult(listResult, 31, 40);
                        AgeResult(listResult, 41, 50);
                        AgeResult(listResult, 61, 70);
                        AgeResult(listResult, 71, 85);

                        Console.WriteLine("Чтобы узнать полные результаты в конкретной возрастой категории введите ее номер (1-5) или 0 для возврата на главное меню.");
                        answer = Console.ReadLine();
                        
                        switch(answer)
                        {
                            case "0":
                                subMenu = false;
                                Menu(listResult, subMenu);
                                break;
                            case "1":
                                SortResult(listResult, 18, 30, true); break;
                            case "2":
                                SortResult(listResult, 31, 40, true); break;
                            case "3":
                                SortResult(listResult, 41, 50, true); break;
                            case "4":
                                SortResult(listResult, 61, 70, true); break;
                            case "5":
                                SortResult(listResult, 71, 85, true); break;
                            default:
                                Console.WriteLine("Ошибка ввода: данного варианта ответа не существует. Возврат в подменю...");
                                Console.ReadKey();
                                Menu(listResult, true);
                                break;
                        }
                        break;

                    case "2":
                        Console.WriteLine("Результаты:");
                        SortResult(listResult, 18, 80, false);
                        break;

                    case "3":
                        Console.WriteLine("Результаты атлетов за всю историю соревнований:");
                        DB.PrintTable();
                        CheckSubMenu(listResult, false);
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine("Ошибка ввода: данного варианта ответа не существует");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void Main()
        {
            string fileName = "../../names.txt";
            string dbName = "../../ResultsDB.db";

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Файла с именами не существует. Закрытие программы...");
                Console.ReadKey();
                return;
            }

            DB = new Database(dbName);
            try
            {
                DB.OpenConnection();

                Console.WriteLine("Соединение с бд установлено. Загружаются новые результаты...");
                Console.ReadKey();

                Random rnd = new Random();
                int numberOfRunners = rnd.Next(5, 10);

                List<Result> listResult = NewResult(numberOfRunners, fileName);

                DB.LoadResults(listResult);

                Console.WriteLine("Результаты загружены в базу данных. Запуск меню...");
                Console.ReadKey();

                Menu(listResult, false);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Ошибка подключения базы данных. Закрытие программы...");
                Console.ReadKey();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }
            finally
            {
                DB.CloseConnection();
            }
        }
    }
}
