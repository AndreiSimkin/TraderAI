using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TraderAI.Bot.Commands
{
    public class Field : ICommand
    {
        const int MaxTree = 4;
        public List<ICommand> Commands { get; set; } = new List<ICommand>();

        public ICommand Clone()
        {
            List<ICommand> commands = new List<ICommand>();
            foreach (ICommand command in Commands)
                commands.Add(command.Clone());
            return new Field() { Commands = commands };
        }


        public void Generate(Random random, double percent, int tree)
        {
            tree++;
            if (tree < MaxTree)
                Entity.GenerateCommands(Commands, random, percent, tree);
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
