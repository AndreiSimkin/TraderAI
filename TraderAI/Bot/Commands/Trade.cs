using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Commands
{
    public class Trade : ICommand
    {
        public long Count { get; set; }

        public ICommand Clone() => new Trade() { Count = Count };

        public void Generate(Entity entity, Random random, double percent, int tree)
        {
            Count += random.Next(-(int)(10 * (percent)), (int)(10 * (percent)));
        }

        public Code Run(Entity entity, Tick tick)
        {
            double price = tick.Price * Count;
            if ((Count > 0 && price <= entity.Trade.Ballance) || (Count < 0 && Count + entity.Trade.Stocks > 0))
            {
                entity.Trade.Ballance -= price;
                entity.Trade.Stocks += Count;
            }
            else if (Count > 0)
            {
                int _count = (int)Math.Floor(entity.Trade.Ballance / tick.Price);
                price = tick.Price * _count;
                entity.Trade.Ballance -= price;
                entity.Trade.Stocks += _count;
            }
            else if (Count < 0)
            {
                price = tick.Price * entity.Trade.Stocks;
                entity.Trade.Ballance += price;
                entity.Trade.Stocks = 0;
            }
            return Code.Continue;
        }
    }
}
