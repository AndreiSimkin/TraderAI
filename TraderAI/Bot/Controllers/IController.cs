using System;
using System.Collections.Generic;
using System.Text;

namespace TraderAI.Bot.Controllers
{
    public interface IController
    {
        Result Verify(Account trade);
    }
}
