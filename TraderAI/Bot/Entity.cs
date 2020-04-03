using System;
using System.Collections.Generic;
using System.Text;
using TraderAI.Bot.Commands;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace TraderAI.Bot
{
    public class Entity : IComparable<Entity>
    {
        public Commands.Field MainField { get; set; }

        public Entity()
        {
        }

        public Entity(Random random, Account trade)
        {
            Init();
            MainField = new Field();
            MainField.Generate(random, 1.0, 0);
            Trade = trade;
        }

        static List<Type> AvailableCommands { get; set; }

        public static void Init()
        {
            if (AvailableCommands == null)
            {
                AvailableCommands = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICommand).IsAssignableFrom(p)).ToList();
                AvailableCommands.Remove(typeof(ICommand));
                AvailableCommands.Remove(typeof(Field));
            }
            
        }

        public List<Commands.Field> Fields { get; set; } = new List<Field>();

        public class Account
        {
            public Account()
            {
            }
         
            public Account(double balance)
            {
                Ballance = balance;
            }

            public double Ballance { get; set; }
            public long Stocks { get; set; } = 0;

            public int Iteration { get; set; } = 0;

            public double Earned { get; private set; }
            public double Spent { get; private set; }

            public double Factor { get => (Earned - Spent) > 0 ? (Earned - Spent) / Iteration : 0; }

            public void Fix(double original)
            {
                Iteration++;
                if (Ballance != original)
                {
                    if (Ballance > original)
                        Earned += Ballance - original;
                    else
                        Spent += original - Ballance;
                }
            }

            public Account Clone(double balance, bool resetiter)
            {
                return new Account() { Ballance = balance, Iteration = resetiter ? 0 : Iteration, Earned = Earned, Spent = Spent };
            }
        }

        public Account Trade { get; set; }

        public void NextStep(Tick tick)
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

        public static ICommand CreateCommand(Random random)
        {
           return (Activator.CreateInstance(AvailableCommands.ElementAt(random.Next(0, AvailableCommands.Count()))) as ICommand);
        }

        public static void GenerateCommands(List<ICommand> commands, Random random, double percent, int tree)
        {
            int iterations = commands.Count > 0 ? (int)Math.Ceiling(commands.Count * percent) : 10;

            for (int i = 0; i < iterations; i++)
            {
                int action = random.Next(0, 3);

                if (action == 0)
                {
                    if (commands.Count < 100)
                    {
                        ICommand command = CreateCommand(random);
                        command.Generate(random, percent, tree);
                        commands.Insert(random.Next(0, commands.Count), command);
                    }
                }
                else if (commands.Count > 0)
                    if (action == 1)
                        commands[random.Next(0, commands.Count)].Generate(random, percent, tree);
                    else if (action == 2)
                        commands.RemoveAt(random.Next(0, commands.Count));
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
