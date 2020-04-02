using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Commands
{
    public interface ICommand
    {
        Code Run(Entity entity, Tick tick);
        void Generate(Random random, double percent, int tree);
        ICommand Clone();
    }
}
