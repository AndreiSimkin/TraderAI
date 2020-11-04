using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands.Analyzers
{
    [Serializable]
    class ChangeValue : IAnalyzer
    {
        double Value { get; set; }
        int Step { get; set; } = -1;
        bool Over { get; set; }

        public bool Analyze(Entity entity, Tick tick)
        {
            if (entity.Memory.Count > 0)
            {
                Tick memtick = entity.Memory.First();
                entity.Memory.RemoveAt(0);
                return Over ? tick.Price - memtick.Price > Value : tick.Price - memtick.Price < Value;
            }
            return false;
        }

        public IAnalyzer Clone()
        {
            return new ChangeValue() { Over = Over, Step = Step, Value = Value };
        }

        public void Generate(Entity entity, Random random, double percent)
        {
            if (percent == 1.0)
                Step = 10 * random.Next(0, 3);
            Value += ((random.NextDouble() - 0.5) * 2.0) / Step;
            if (percent - random.NextDouble() >= 0)
                Over = random.Next(0, 2) == 1;
        }
    }
}
