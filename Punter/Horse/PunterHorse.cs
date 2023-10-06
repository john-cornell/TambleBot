using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot.Punter.Horse
{
    public class PunterHorse
    {
        public string Description { get; set; }
        public List<PunterHorseStats> Stats { get; set; } = new List<PunterHorseStats>();
    }
}
