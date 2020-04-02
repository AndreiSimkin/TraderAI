using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot
{

    [Serializable]
    public struct Tick
    {
        public long Time { get; set; }
        public double Price { get; set; }

        public Tick(long time, double price)
        {
            Time = time;
            Price = price;
        }
    }
}
