using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot
{
    public class Results
    {
        public List<Result> ResultsList { get; set; } = new List<Result>();
        public List<Error> ErrorsList { get; set; } = new List<Error>();
    }
}
