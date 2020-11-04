using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Controllers
{
    [Serializable]
    public class Inflation : IController
    {
        int Iteration { get; set; }
        double AllowInflation { get; set; }
        double Level { get; set; }
        double Speed { get; set; }

        public Inflation(int iteration, double allowinflation, double speed) { Iteration = iteration; AllowInflation = allowinflation; Speed = speed;  }

        public Result Verify(Account trade)
        {

            if (trade.Iteration > Iteration  && trade.Average > Level)
                Level += (trade.Average - Level) * Speed;
            return trade.Iteration < Iteration ? Result.Verified : 1.0 - (trade.Average / Level) <= AllowInflation ? Result.Verified : Result.Rejected;
        }
    }
}
