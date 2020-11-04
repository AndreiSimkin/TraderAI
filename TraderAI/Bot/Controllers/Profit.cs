using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Controllers
{
    [Serializable]
    public class Profit : IController
    {
        double Min { get; set; }
        int Iteration { get; set; }

        public Profit(int iteration, double min) { Iteration = iteration; Min = min; }

        public Result Verify(Account trade)
        {
            return trade.Iteration < Iteration ? Result.Unknown : trade.Average >= Min ? Result.Verified : Result.Rejected;
        }
    }
}
