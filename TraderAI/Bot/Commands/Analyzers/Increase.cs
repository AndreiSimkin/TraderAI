using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands.Analyzers
{
    public class Increase : IAnalyzer
    {
        public bool Analyze(Entity entity, Tick tick)
        {
            if (entity.Memory.Count > 0)
            {
                Tick memtick = entity.Memory.First();
                entity.Memory.RemoveAt(0);
                return tick.Price > memtick.Price;
            }
            return false;
        }

        public IAnalyzer Clone()
        {
            return new Сhange();
        }

        public void Generate(Entity entity, Random random, double percent) { }
    }
}
