using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Commands
{
    public class Wait : ICommand
    {
        public int Ticks { get; set; } = -1;
        public bool Waiting { get => Timer > 0; }
        public int SubSteps { get; set; } = 0;

        int Timer { get; set; } = -1;
        public ICommand Clone()
        {
            return new Wait() { Ticks = Ticks };
        }

        public void Generate(Random random, double percent, int tree)
        {
            if (Ticks == -1)
                Ticks = random.Next(1, 1000);
            else
                Ticks += random.Next(-(int)Math.Ceiling(Ticks * percent), (int)Math.Ceiling(Ticks * percent));
            while (Ticks < 0)
                Ticks = 1000 - Ticks;
        }

        public void Start(Entity entity)
        {
            Timer = Ticks;
        }

        public void Reset()
        {
            Timer = -1;
        }

        public Code Run(Entity entity, Tick tick)
        {
            if (Timer == -1)
                Start(entity);
            Timer--;
            if (Timer == 0)
            {
                Reset();
                return Code.Continue;
            }
            else
                return Code.Skip;
        }
    }
}
