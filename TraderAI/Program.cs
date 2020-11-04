using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TraderAI
{
    class Program
    {
        static BinaryFormatter Formatter { get; set; } = new BinaryFormatter();

        static void SaveObject(string path, object data)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            Formatter.Serialize(stream, data);
            stream.Close();
        }

        static object LoadObject(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            object data = Formatter.Deserialize(stream);
            stream.Close();
            return data;
        }

        static DateTime LastDownload { get; set; }

        enum Market { MOEX = 10003 }

        static List<Bot.Tick> GetHistory(DateTime date, Market market, string code)
        {
            Directory.CreateDirectory("HistoryData/" + market + "/" + code);
            if (File.Exists("HistoryData/" + market + "/" + code + "/" + date.ToShortDateString().Replace('.', '_') + ".ticks"))
                return (List<Bot.Tick>)LoadObject("HistoryData/" + market + "/" + code + "/" + date.ToShortDateString().Replace('.', '_') + ".ticks");
            else
            {
                TimeSpan time = new TimeSpan(DateTime.Now.Ticks - LastDownload.Ticks);
                if (time.Milliseconds < 1000)
                    System.Threading.Thread.Sleep(1000 - time.Milliseconds);
                List<Bot.Tick> ticks = new List<Bot.Tick>();
                WebClient web = new WebClient();
                string url = $"http://export.finam.ru/market={((int)market).ToString().Split(new string[] { "000" }, StringSplitOptions.RemoveEmptyEntries)[0]}&em={((int)market).ToString().Split(new string[] { "000" }, StringSplitOptions.RemoveEmptyEntries)[1]}&code={code}&apply=0&df={date.Day}&mf={date.Month - 1}&yf={date.Year}&from={date.ToShortDateString()}&dt={date.Day}&mt={date.Month - 1}&yt={date.Year}&to={date.ToShortDateString()}&p=1&cn={code}&dtf=4&tmf=3&MSOR=1&mstime=on&mstimever=1&sep=3&sep2=3&datf=10&fsp=1";
                string[] data = web.DownloadString(url).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                LastDownload = DateTime.Now;
                if (data.Length > 1)
                    foreach (string tick in data)
                    {
                        string[] split = tick.Split(new char[] { ';' });
                        ticks.Add(new Bot.Tick(DateTime.Parse(split[0].Replace('/', '.') + " " + split[1]).Ticks, double.Parse(split[2].Replace('.', ','))));
                    }

                SaveObject("HistoryData/" + market + "/" + code + "/" + date.ToShortDateString().Replace('.', '_') + ".ticks", ticks);

                return ticks;
            }
        }

        static void Main()
        {
            /*
            for (int i = 0; i < 366 * 13; i++)
            {
                var ticks = GetHistory(CurrentDate, Market.MOEX, "SBER"); 
                if (CurrentDate.DayOfWeek == DayOfWeek.Monday)
                    CurrentDate = CurrentDate.AddDays(-3);
                else
                    CurrentDate = CurrentDate.AddDays(-1);
                GC.Collect();
            }*/

            Directory.CreateDirectory("Bots");

            DateTime CurrentDate = DateTime.Parse("17.03.2009");
            var Ticks = GetHistory(CurrentDate, Market.MOEX, "SBER");
            Random random = new Random();
            List<Bot.Entity> entitiesgen = new List<Bot.Entity>();

            double EBalance = 100000;
            int GTicks = Ticks.Count / 10;
            double GPercent = 0.3;
            int ECount = 45;
            int MinIter = 10;
            double MinAvange = 1;
            double Inflation = 0.2;


            for (int i = entitiesgen.Count; i < 10; i++)
                entitiesgen.Add(new Bot.Entity(random, new Bot.Account(EBalance,
                    new List<Bot.Controllers.IController>()
                    {
                        new Bot.Controllers.Profit(MinIter, MinAvange),
                        new Bot.Controllers.Inflation(100, Inflation, 0.01)
                    }
                    )));

            //   bool GenerateNew = true;

            double MaxBalance = 0;
            int MaxIter = 0;
            double MaxAvange = 0;
            int tick = 0;


            while (true)
            {
                for (int i = 0; i < GTicks; i++, tick++)
                {
                    if (tick >= Ticks.Count)
                    {
                        do
                        {
                            if (CurrentDate.DayOfWeek == DayOfWeek.Friday)
                                CurrentDate = CurrentDate.AddDays(3);
                            else
                                CurrentDate = CurrentDate.AddDays(1);
                            Ticks.Clear();
                            Ticks = GetHistory(CurrentDate, Market.MOEX, "SBER");
                            GC.Collect(GC.MaxGeneration);
                        } while (Ticks.Count < 100000);
                        tick = 0;
                        GTicks = Ticks.Count / 10;
                    }
                    foreach (Bot.Entity entity in entitiesgen)
                        entity.Run(Ticks[tick]);
                }

                List<Bot.Entity> newentitiesgen = new List<Bot.Entity>();
                List<Bot.Entity> clones = new List<Bot.Entity>();

                entitiesgen.Sort();
                for (int i = entitiesgen.Count - 1; i >= 0; i--)
                {
                    switch (entitiesgen[i].Trade.Verify())
                    {
                        case Bot.Controllers.Result.Verified:
                            if (MaxAvange < entitiesgen[i].Trade.Average)
                                MaxAvange = entitiesgen[i].Trade.Average;
                            if (MaxIter < entitiesgen[i].Trade.Iteration)
                                MaxIter = entitiesgen[i].Trade.Iteration;
                            newentitiesgen.Add(entitiesgen[i].Clone(EBalance));
                            clones.Insert(0, entitiesgen[i].Clone(EBalance, true));
                            clones[0].Generate(random, GPercent);
                            break;
                        case Bot.Controllers.Result.Unknown:
                            if (newentitiesgen.Count < ECount)
                                clones.Add(entitiesgen[i].Clone(EBalance));
                            break;
                        case Bot.Controllers.Result.Rejected:
                            if (entitiesgen[i].Trade.Iteration > 300)
                            {
                                SaveObject("Bots/" + entitiesgen[i].Trade.Iteration + "_" + entitiesgen[i].Trade.Average.ToString().Replace(',', '_') + ".bot", entitiesgen[i].Clone(EBalance));
                            }
                            break;
                    }
                }

                if (clones.Count > 0 && newentitiesgen.Count < ECount)
                    newentitiesgen.AddRange(clones.GetRange(0, ECount - newentitiesgen.Count > clones.Count ? clones.Count : ECount - newentitiesgen.Count));


                Console.SetCursorPosition(0, 0);
                for (int i = entitiesgen.Count - 1; i >= 0; i--)
                    Console.WriteLine("#" + (entitiesgen.Count - i) + "(" + entitiesgen[i].Trade.Iteration + ")(f" + (entitiesgen[i].Trade.Iteration > 10 ? Math.Round(entitiesgen[i].Trade.Average, 3).ToString() : "?") + "): Balance: " + entitiesgen[i].Trade.Ballance + "Р                             ");
                for (int i = 0; i < 10; i++)
                    Console.WriteLine("                                                                      ");

                Console.Title = "Date: " + CurrentDate.ToShortDateString() + ", Max iteration: " + MaxIter + ", Max avange: " + MaxAvange + ", Max ballance: " + MaxBalance + "Р";

                if (newentitiesgen.Count == 0)
                {
                    for (int i = newentitiesgen.Count; i < 10; i++)
                        newentitiesgen.Add(new Bot.Entity(random, new Bot.Account(EBalance,
                            new List<Bot.Controllers.IController>()
                            {
                                new Bot.Controllers.Profit(MinIter, MinAvange),
                                new Bot.Controllers.Inflation(100, Inflation, 0.01)
                            }
                            )));
                    CurrentDate = DateTime.Parse("17.03.2009");
                    Ticks.Clear();
                    Ticks = GetHistory(CurrentDate, Market.MOEX, "SBER");
                    GC.Collect(GC.MaxGeneration);
                    tick = 0;
                }

                entitiesgen.Clear();
                entitiesgen = newentitiesgen;
            }

        }
    }
}
