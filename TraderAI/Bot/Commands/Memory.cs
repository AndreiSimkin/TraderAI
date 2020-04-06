using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands
{
    public class Memory : ICommand
    {
        public ICommand Clone() => new Memory() { Insert = Insert };

        public void Generate(Entity entity, Random random, double percent, int tree) => Insert = random.Next(0, 2) == 0;

        bool Insert { get; set; }

        const int Max = 100;

        public Code Run(Entity entity, Tick tick)
        {
            if (Insert)
            {
                entity.Memory.Insert(0, tick);
                if (entity.Memory.Count > Max)
                    entity.Memory.RemoveAt(entity.Memory.Count - 1);
            }
            else
            {
                entity.Memory.Add(tick);
                if (entity.Memory.Count > Max)
                    entity.Memory.RemoveAt(0);
            }
            return Code.Continue;
        }
    }
}
