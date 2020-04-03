using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands.Analyzers
{
    public class Сhange : IAnalyzer
    {
        int Step { get; set; } = -1;
        double Value { get; set; }

        public bool Analyze(Entity entity, Tick tick)
        {
            if (entity.Memory.Count > 0)
            {
                Tick memtick = entity.Memory.First();
                entity.Memory.RemoveAt(0);
                return Value > 0 ? memtick.Price / tick.Price > Value : tick.Price / memtick.Price > -1 * Value;
            }
            return false;
        }

        public IAnalyzer Clone()
        {
            return new Сhange() { Step = Step, Value = Value };
        }

        public void Generate(Entity entity, Random random, double percent)
        {
            if (percent == 1.0)
                Step = (int)Math.Pow(10, random.Next(0, 3));
            Value += ((random.NextDouble() - 0.5) * 2.0) / Step;
        }
    }
}
