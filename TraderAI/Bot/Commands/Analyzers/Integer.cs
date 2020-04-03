using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Commands.Analyzers
{
    class Integer : IAnalyzer
    {
        double Value { get; set; }
        int Step { get; set; } = -1;

        public bool Analyze(Entity entity, Tick tick)
        {
            return Value > 0 ? tick.Price >= Value : tick.Price <= -1 * Value;
        }

        public IAnalyzer Clone()
        {
            return new Integer() { Step = Step, Value = Value };
        }

        public void Generate(Entity entity, Random random, double percent)
        {
            if (percent == 1.0)
                Step = 10 * random.Next(1, 4);
            Value += ((random.NextDouble() - 0.5) * 2.0) * Step;
        }
    }
}
