using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot.Punter.Trainer
{
    public class PunterTrainer
    {
        public string Description { get; set; }
        public List<PunterTrainerStats> Stats { get; set; } = new List<PunterTrainerStats>();
    }
}
