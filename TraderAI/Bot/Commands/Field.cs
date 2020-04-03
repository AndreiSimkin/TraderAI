using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands
{
    public class Field : ICommand
    {
        const int MaxTree = 5;
        public List<ICommand> Commands { get; set; } = new List<ICommand>();

        public ICommand Clone()
        {
            List<ICommand> commands = new List<ICommand>();
            foreach (ICommand command in Commands)
                commands.Add(command.Clone());
            return new Field() { Commands = commands };
        }

        static List<Type> AvailableCommands { get; set; }

        public void Generate(Entity entity, Random random, double percent, int tree)
        {
            tree++;
            if (tree < MaxTree)
            {
                if (AvailableCommands == null)
                {
                    AvailableCommands = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => typeof(ICommand).IsAssignableFrom(p)).ToList();
                    AvailableCommands.Remove(typeof(ICommand));
                    AvailableCommands.Remove(typeof(Field));
                }

                int iterations = Commands.Count > 0 ? (int)Math.Ceiling(Commands.Count * percent) : (int)Math.Ceiling(10 * percent);

                for (int i = 0; i < iterations; i++)
                {
                    int action = random.Next(0, 3);

                    if (action == 0)
                    {
                        if (Commands.Count < 100)
                        {
                            ICommand command = (Activator.CreateInstance(AvailableCommands.ElementAt(random.Next(0, AvailableCommands.Count()))) as ICommand);
                            command.Generate(entity, random, 1.0, tree);
                            Commands.Insert(random.Next(0, Commands.Count), command);
                        }
                    }
                    else if (Commands.Count > 0)
                        if (action == 1)
                            Commands[random.Next(0, Commands.Count)].Generate(entity, random, percent, tree);
                        else if (action == 2)
                            Commands.RemoveAt(random.Next(0, Commands.Count));
                }
            }
        }

        public Code Run(Entity entity, Tick tick)
        {
            if (entity.Fields.Count > 0)
                entity.Fields.First().Commands.RemoveAt(0);
            entity.Fields.Insert(0, (Field)Clone());
            return Code.Add;
        }
    }
}
