using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TraderAI.Bot
{

    [Serializable]
    public class Tick : IEquatable<Tick>
    {
        public long Time { get; set; }
        public double Price { get; set; }

        public Tick(long time, double price)
        {
            Time = time;
            Price = price;
        }

        public bool Equals([AllowNull] Tick other) => Time == other.Time && Price == other.Price;
    }
}
