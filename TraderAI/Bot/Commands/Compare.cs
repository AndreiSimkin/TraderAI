using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Commands
{
    public class Compare : ICommand
    {
        public double Value { get; set; }
        public int Step { get; set; } = -1;
        public int SubSteps { get; set; } = 2;

        public ICommand Clone()
        {
            return new Compare() { Step = Step, Value = Value, FailField = (Field)FailField.Clone(), OKField = (Field)OKField.Clone() };
        }

        Field OKField { get; set; }
        Field FailField { get; set; }

        public void Generate(Random random, double percent, int tree)
        {
            if (percent == 1.0)
                Step = random.Next(1, 1000);
            Value += ((random.NextDouble() - 0.5) * 2.0) * Step;

            if (OKField == null)
            {
                OKField = new Field();
                FailField = new Field();
            }

            OKField.Generate(random, percent, tree);
            FailField.Generate(random, percent, tree);
        }

        public Code Run(Entity entity, Tick tick)
        {
            if (tick.Price > Value)
                OKField.Run(entity, tick);
            else
                FailField.Run(entity, tick);
            return Code.Add;
        }
    }
}
