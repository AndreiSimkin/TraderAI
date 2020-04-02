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
        static List<Bot.Tick> GetHistory(DateTime date)
        {
            List<Bot.Tick> ticks = new List<Bot.Tick>();
            WebClient web = new WebClient();
            string url = $"http://export.finam.ru/market=517&em=419805&code=SPBEX.AAPL&apply=0&df={date.Day}&mf={date.Month}&yf={date.Year}&from={date.ToShortDateString()}&dt={date.Day}&mt={date.Month}&yt={date.Year}&to={date.ToShortDateString()}&p=1&cn=SPBEX.AAPL&dtf=4&tmf=3&MSOR=1&mstime=on&mstimever=1&sep=3&sep2=3&datf=10&fsp=1";
            string[] data = web.DownloadString(url).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string tick in data)
            {
                string[] split = tick.Split(new char[] { ';' });
                ticks.Add(new Bot.Tick(DateTime.Parse(split[0].Replace('/', '.') + " " + split[1]).Ticks, double.Parse(split[2].Replace('.', ','))));
            }

            return ticks;
        }

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

        static void Main(string[] args)
        {
            var Ticks = (List<Bot.Tick>)LoadObject("12_02_2020.ticks");
            Random random = new Random();
            List<Bot.Entity> entitiesgen = new List<Bot.Entity>();

            double EBalance = 10000;
            int GTicks = Ticks.Count / 24;
            double GPercent = 0.3;
            int ECount = 15;

            for (int i = entitiesgen.Count; i < ECount; i++)
                entitiesgen.Add(new Bot.Entity(random, new Bot.Entity.Account(EBalance)));

            bool GenerateNew = true;

            double MaxFactor = 0;
            double MaxBalance = 0;
            int MaxIter = 0;
            int tick = 0;

            int MinIter = 10;

            double AverageFactor = 1;

            while (true)
            {
                for (int i = 0; i < GTicks; i++, tick++)
                {
                    if (tick >= Ticks.Count)
                        tick = 0;
                    foreach (Bot.Entity entity in entitiesgen)
                        entity.NextStep(Ticks[tick]);
                }

                entitiesgen.Sort();

                if (entitiesgen.Last().Trade.Ballance > MaxBalance)
                    MaxBalance = entitiesgen.Last().Trade.Ballance;

                if (entitiesgen.Last().Trade.Ballance > MaxBalance)
                    MaxBalance = entitiesgen.Last().Trade.Ballance;

                for (int i = 0; i < entitiesgen.Count; i++)
                    if (entitiesgen[i].Trade.Factor > MaxFactor)
                        MaxFactor = entitiesgen[i].Trade.Factor;

                List<Bot.Entity> newentitiesgen = new List<Bot.Entity>();

                MaxFactor = 0;
                for (int i = 0; i < entitiesgen.Count; i++)
                {
                    entitiesgen[i].Trade.Fix(EBalance);

                    if (entitiesgen[i].Trade.Iteration >= MinIter)
                    {
                        if (entitiesgen[i].Trade.Factor > AverageFactor)
                        {
                            if (entitiesgen[i].Trade.Factor > MaxFactor)
                                MaxFactor = entitiesgen[i].Trade.Factor;
                            if (entitiesgen[i].Trade.Iteration > MaxIter)
                                MaxIter = entitiesgen[i].Trade.Iteration;

                            newentitiesgen.Add(entitiesgen[i].Clone(EBalance));
                            if (ECount > entitiesgen.Count)
                            {
                                newentitiesgen.Add(entitiesgen[i].Clone(EBalance, true));
                                newentitiesgen.Last().MainField.Generate(random, GPercent, 0);
                            }
                        }
                    }
                    else
                        newentitiesgen.Add(entitiesgen[i].Clone(EBalance));
                }

                Console.SetCursorPosition(0, 0);
                for (int i = entitiesgen.Count - 1; i >= 0; i--)
                    Console.WriteLine("#" + (entitiesgen.Count - i) + "(" + entitiesgen[i].Trade.Iteration + ")(f" + (entitiesgen[i].Trade.Iteration > 10 ? Math.Round(entitiesgen[i].Trade.Factor, 3).ToString() : "?")  + "): Balance: " + entitiesgen[i].Trade.Ballance + "$                             ");
                for (int i = 0; i < 10; i++)
                    Console.WriteLine("                                                                      ");

                Console.Title = "Max iteration: " + MaxIter + ", Max factor: " + MaxFactor + ", Max ballance: " + MaxBalance + "$";

                if (newentitiesgen.Count == 0)
                    for (int i = newentitiesgen.Count / 2; i < ECount; i++)
                        newentitiesgen.Add(new Bot.Entity(random, new Bot.Entity.Account(EBalance)));

                entitiesgen.Clear();
                entitiesgen = newentitiesgen;

            }
        }
    }
}
