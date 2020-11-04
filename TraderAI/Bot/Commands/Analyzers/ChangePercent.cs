using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands.Analyzers
{
    [Serializable]
    public class ChangePercent : IAnalyzer
    {
        int Step { get; set; } = -1;
        double Value { get; set; }
        bool Over { get; set; }

        public bool Analyze(Entity entity, Tick tick)
        {
            if (entity.Memory.Count > 0)
            {
                Tick memtick = entity.Memory.First();
                entity.Memory.RemoveAt(0);
                return Value > 0 ? (Over ? memtick.Price / tick.Price > Value : memtick.Price / tick.Price < Value) : (Over ? tick.Price / memtick.Price > -1 * Value : tick.Price / memtick.Price < -1 * Value);
            }
            return false;
        }

        public IAnalyzer Clone()
        {
            return new ChangePercent() { Step = Step, Value = Value };
        }

        public void Generate(Entity entity, Random random, double percent)
        {
            if (percent == 1.0)
                Step = (int)Math.Pow(10, random.Next(0, 3));
            if (percent - random.NextDouble() >= 0)
                Over = random.Next(0, 2) == 1;
            Value += ((random.NextDouble() - 0.5) * 2.0) / Step;
        }
    }
}
