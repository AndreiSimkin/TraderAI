using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Commands.Analyzers
{
    public interface IAnalyzer
    {
        bool Analyze(Entity entity, Tick tick);
        void Generate(Entity entity, Random random, double percent);
        IAnalyzer Clone();
    }
}
