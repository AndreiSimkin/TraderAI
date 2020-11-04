using System;
using System.Collections.Generic;
using System.Text;
using TraderAI.Bot.Commands;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace TraderAI.Bot
{
    [Serializable]
    public class Entity : IComparable<Entity>
    {
        public Commands.Field MainField { get; set; }

        public Entity() { }

        public Entity(Random random, Account trade)
        {
            MainField = new Field();
            MainField.Generate(this, random, 1.0, 0);
            Trade = trade;
        }

        public List<Tick> Memory { get; set; } = new List<Tick>();

        public List<Commands.Field> Fields { get; set; } = new List<Field>();
       
        public Account Trade { get; set; }

        public void Generate(Random random, double percent)
        {
            MainField.Generate(this, random, percent, 0);
        }

        public void Run(Tick tick)
        {
            if (Fields.Count == 0)
                MainField.Run(this, tick);

            Code code = Code.Continue;

            while (Fields.Count > 0 && code != Code.Skip)
            {
                while (Fields.First().Commands.Count > 0 && code != Code.Skip)
                {
                    code = Fields.First().Commands[0].Run(this, tick);
                    if (code == Code.Continue)
                        Fields.First().Commands.RemoveAt(0);
                }
                if (Fields.First().Commands.Count == 0)
                    Fields.RemoveAt(0);
            }
        }

        public Entity Clone(double balance, bool newgen = false)
        {
            return new Entity() { Trade = Trade.Clone(balance, newgen), MainField = (Field)MainField.Clone() };
        }

        public int CompareTo(Entity other)
        {
            if (Trade.Iteration > other.Trade.Iteration)
                return 1;
            if (Trade.Iteration < other.Trade.Iteration)
                return -1;
            else
                return 0;
        }
    }
}
