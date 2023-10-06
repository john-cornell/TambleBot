using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TambleBot.PromptBuilder;

namespace TambleBot.Punter.Predictors
{
    public class ProsAndConsPredictor : PunterPredictor
    {
        
        public override async Task<string> Predict(RaceData data, string model)
        {
            var raceText = GetRaceDataText(data);

            string prosAndConsPrompt =
                RaceDataPrompts.BuildProsAndConsPrompt(raceText);

            var prosAndConsResponse = await AskBot(prosAndConsPrompt, model: model);

            string prosAndCons = prosAndConsResponse.ResultsList[0].Response;

            Log(prosAndCons);

            string prompt = 
                RaceDataPrompts.BuildProsAndConsBasedPrompt(raceText, prosAndCons);

            Log("Predicting");

            var prediction = await AskBot(prompt, model: model);

            return prediction.ResultsList[0].Response;
        }
    }
}
