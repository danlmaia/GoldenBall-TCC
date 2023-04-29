using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenBall_TCC.Classes
{
    public class Team
    {
        List<Player> Players { get; set; }

        Player captain { get ; set; }

        TrainingMethod TrainingMethod { get; set; }


    }
}
