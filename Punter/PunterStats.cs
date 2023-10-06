using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot.Punter
{
    public abstract class PunterStats
    {
        public List<string> Headers { get; set; } = new List<string>();
        public List<List<string>> Data { get; set; } = new List<List<string>>();

        public override string ToString()
        {
            string headers = string.Join(",", Headers);
            string data = string.Join("\n", Data.Select(x => string.Join(",", x)));

            return $"{headers}\n{data}";
        }
    }
}
