using System;
using System.Collections.Generic;
using System.Text;
using TraderAI.Bot.Controllers;

namespace TraderAI.Bot
{
    [Serializable]
    public class Account
    {
        public Account()
        {
        }

        public Account(double balance, List<IController> controllers)
        {
            Ballance = balance;
            StartBallance = balance;
            Controllers = controllers;
        }

        List<IController> Controllers { get; set; }

        double StartBallance { get; set; }
        public double Ballance { get; set; }
        public long Stocks { get; set; } = 0;

        public int Iteration { get; set; } = 0;

        public double Earned { get; private set; }
        public double Spent { get; private set; }

        public double Average { get => (Earned - Spent) > 0 ? (Earned - Spent) / Iteration : 0; }

        public Result Verify()
        {
            Iteration++;
            if (Ballance != StartBallance)
            {
                if (Ballance > StartBallance)
                    Earned += Ballance - StartBallance;
                else
                    Spent += StartBallance - Ballance;
            }

            Result result = Result.Verified;

            for (int i = 0; i < Controllers.Count && result == Result.Verified; i++)
                result = Controllers[i].Verify(this);

            return result;
        }

        public Account Clone(double balance, bool resetiter)
        {
            return new Account() { Controllers = Controllers, Ballance = balance, StartBallance = balance, Iteration = resetiter ? 0 : Iteration, Earned = resetiter ? 0 : Earned, Spent = resetiter ? 0 : Spent };
        }
    }
}
