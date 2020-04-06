using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot
{
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
            return new Account() { Ballance = balance, Iteration = resetiter ? 0 : Iteration, Earned = resetiter ? 0 : Earned, Spent = resetiter ? 0 : Spent };
        }
    }
}
