using GPTNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TambleBot.PromptBuilder;

namespace TambleBot.Punter.Predictors
{
    public class BasicPunterPredictor : PunterPredictor
    {
        public override async Task<string> Predict(RaceData data, string model)
        {
            var raceText = GetRaceDataText(data);

            string prompt =
                RaceDataPrompts.BuildPrompt(raceText, providedAnalysisMethod: null);

            Log("Predicting");

            return  (AskBot(prompt, model: model).Result.ResultsList[0].Response);
        }
    }
}
