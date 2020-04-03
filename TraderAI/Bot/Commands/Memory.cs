using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands
{
    public class Memory : ICommand
    {
        public ICommand Clone() => new Memory() { Insert = Insert };

        public void Generate(Entity entity, Random random, double percent, int tree) => Insert = random.Next(0, 2) - 1 == -1;

        bool Insert { get; set; }

        public Code Run(Entity entity, Tick tick)
        {
            if (Insert)
                entity.Memory.Insert(0, tick);
            else
                entity.Memory.Add(tick);
            return Code.Continue;
        }
    }
}
