using System;
using System.Collections.Generic;
using System.Text;
using TraderAI.Bot.Commands.Analyzers;
using System.Linq;

namespace TraderAI.Bot.Commands
{
    
    public class Analyze : ICommand
    {
        double test;
        public ICommand Clone() => new Analyze() { Analyzer = Analyzer.Clone(), FailField = (Field)FailField.Clone(), OKField = (Field)OKField.Clone() };

        Analyzers.IAnalyzer Analyzer { get; set; }
        Field OKField { get; set; }
        Field FailField { get; set; }

        static List<Type> AvailableAnalyzers { get; set; }

        public void Generate(Entity entity, Random random, double percent, int tree)
        {

            if (OKField == null)
            {
                OKField = new Field();
                FailField = new Field();
            }

            OKField.Generate(entity, random, percent, tree);
            FailField.Generate(entity, random, percent, tree);

            if (AvailableAnalyzers == null)
            {
                AvailableAnalyzers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IAnalyzer).IsAssignableFrom(p)).ToList();
                AvailableAnalyzers.Remove(typeof(IAnalyzer));
            }

            if (Analyzer == null)
                test = percent;

            if (percent - random.NextDouble() >= 0)
            {
                Analyzer = (Activator.CreateInstance(AvailableAnalyzers.ElementAt(random.Next(0, AvailableAnalyzers.Count()))) as IAnalyzer);
                Analyzer.Generate(entity, random, percent);
            }
            else
            {
                Analyzer.Generate(entity, random, percent);
            }
        }

        public Code Run(Entity entity, Tick tick)
        {
            if (Analyzer.Analyze(entity, tick))
                OKField.Run(entity, tick);
            else
                FailField.Run(entity, tick);
            return Code.Add;
        }
    }
}
