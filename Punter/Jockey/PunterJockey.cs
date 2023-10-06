using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot.Punter.Jockey
{
    public class PunterJockey
    {
        public string Description { get; set; }
        public List<PunterJockeyStats> Stats { get; set; } = new List<PunterJockeyStats>();
    }
}
