using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot.Punter.Predictors
{
    public abstract class PunterPredictor : Bot
    {
        public abstract Task<string> Predict(RaceData data, string model);

        protected string GetRaceDataText(RaceData data)
        {
            StringBuilder promptBuilder = new StringBuilder();

            promptBuilder.AppendLine($"Track Condition: {data.TrackCondition}");
            promptBuilder.AppendLine(string.Join('\n', data.DataLines));

            promptBuilder.AppendLine("Jockey Data: ");
            promptBuilder.AppendLine(data.JockeyDescriptions.ToString());


            promptBuilder.AppendLine("Trainer Data: ");
            promptBuilder.AppendLine(data.TrainerDescriptions.ToString());


            promptBuilder.AppendLine("Horse Data: ");
            promptBuilder.AppendLine(data.HorseDescriptions.ToString());
            return promptBuilder.ToString();
        }

        protected void Log(string message, bool addEllipses = true)
        {
            Console.WriteLine();
            Console.Write(message);

            if (addEllipses)
            {
                Console.Write(" ...");
            }

            Console.WriteLine();
        }
    }
}
