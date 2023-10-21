using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot.PromptBuilder
{
    public static class RaceDataPrompts
    {
        public static string BuildAnalysisMethodPrompt(string text)
        {
            string prompt = "Here is some data about a race that I want you to predict the winning horses in the correct order, using your best and most accurate data analysis skills. Without actually making any predictions what would be the most accurate way to analyse this data?\n\n" + text + "\n\nAnswer:";

            return prompt;
        }

        public static string BuildExpertsDescriptionsPrompt(string raceText)
        {
            return
                BuildExpertsPrompt() + 
                $"Your job is to describe the four members opinions, to give their expert viewpoints on how best the race will be predicted. {Environment.NewLine}" +
                $"The Race Data is as follows: {Environment.NewLine}" +
                raceText + Environment.NewLine + 
                $"Three important rules: 1) Only provide the method of analysis, not to prediction. Do not give a prediction on horses only method of analysis 2) The experts can ONLY work with the data provided above, so don't task them to to work with data not given, and 3) your output MUST be valid JSON and in the format: {Environment.NewLine} " +
                "{\"Experts\":[{ \"Opinion\":\"Expert Opinion\"},{ \"Opinion\":\"Expert Opinion\"},{ \"Opinion\":\"Expert Opinion\"},{ \"Opinion\":\"Expert Opinion\"},]}";
        }

        private static string BuildExpertsPrompt()
        {
            return $"Task: A committee of experts will be created to determine to outcome of a horse race, to accurately predict the outcome of the race, and output the winning horses in the correct order. The predicted outcome must be as close to the real outcome as possible. This is paramount. " +
                   $"On this panel there will be 4 separate members, each providing a different viewpoint on how the analysis should best be carried out. This analysis can only be done on the data provided, and should take into account as many factors and data features as possible.";
        }

        public static string BuildExpertPrompt(string raceText, string method)
        {
            return
                BuildExpertsPrompt() + Environment.NewLine +
                $"You are on this committee and are an expert in your field. Your method of analysis is as follows {method}. This is how you will view the race. " +
                Environment.NewLine +
                $"Using the following race data determine, to your utmost certainty, the order of the horses to win this race. If you choose the first four horses in the data only, especially in that order, you probably made a mistake so don't do that." +
                Environment.NewLine +
                
                $"Your output MUST be valid JSON and in the format: {Environment.NewLine} " +
                "CanChoose:{bool - yes or no whether you are relatively confident that the race can be called}" +
                "HorseToWin: {ONLY PUT HORSE NUMBER AND NAME}" +
                "HorseToRunSecond: {ONLY PUT HORSE NUMBER AND NAME}" +
                "HorseToRunThird: {ONLY PUT HORSE NUMBER AND NAME}" +
                "HorseToRunFourth: {ONLY PUT HORSE NUMBER AND NAME}" +
                "Analysis: {Use this area to perform any step by step thinking and analysis you need to do}" +
                "Explanation: {Explain your reasoning for your prediction}" +
                "Confidence: {A relative idea of confidence in prediction }" + Environment.NewLine +
                $"The race data is (never ever assume the answer has anything to do with the order of the horses in the data, though barrier may be a factor): " + Environment.NewLine +
                raceText + Environment.NewLine + "Answer: ";
        }

        public static string BuildAdjudicatorPrompt(string raceText, List<string> methods, List<string> outputs)
        {
            return BuildExpertsPrompt() + Environment.NewLine +
                   
                   "The four experts are looking at the race as follows: " + Environment.NewLine +
                     string.Join(Environment.NewLine, methods) + Environment.NewLine +
                   "The four experts have given these responses: " + Environment.NewLine +
                   string.Join(Environment.NewLine, outputs) + Environment.NewLine +
                   "To your best ability, as an expert your role is to make a decision on the outcome the race. Decide if you trust the analysis of each expert, confirm the data they claim they are using is valid from the race data, take into account these viewpoints, and your own expert opinion the horse to win this race" + Environment.NewLine +
                   $"Your output MUST be valid JSON and in the format: {Environment.NewLine} " +
                   "CanChoose:{bool - yes or no whether you are relatively confident that the race can be called}" +
                   "FirstPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                   "SecondPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                   "ThirdPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                   "FourthPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                   "Analysis: {Use this area to perform any step by step thinking and analysis you need to do}" +
                   "Explanation: {Explain your reasoning for your prediction}" +
                   "Confidence: {A relative idea of confidence in prediction }" + Environment.NewLine +
                   "This is the Race Data (never ever assume the answer has anything to do with the order of the horses in the data, though barrier may be a factor): " + Environment.NewLine +
                   raceText + Environment.NewLine +
                   "The horses to win this race, in place order will be: ";
        }

        public static string BuildPrompt(string text, string providedAnalysisMethod = null)
        {
            string method = providedAnalysisMethod == null ? "" : " You have determined the best method to analyze this data is " + providedAnalysisMethod + ".";

            string prompt =
                $"Task: Here is some data about a race that I want you to predict using your best and most accurate data analysis skills.Based on the data given, analyse the following data as an expert horse race expert and data analyst and give your prediction on the race. {providedAnalysisMethod}Feel free to answer the race is too hard to call, but do your best to provide an answer. {Environment.NewLine} " +
                "Never ever make up statistics, only use the data you have. That is very very important. " +
                "Never ever assume the answer has anything to do with the order of the horses, though barrier may be a factor. " +
                "Consider all the race data presented to you, and how all the factors may influence each other, be creative with your analysis to get the best outcome. " +
                $"It is important that you ALWAYS Format your answer in json with the following fields: {Environment.NewLine}" +
                "CanChoose:{bool - yes or no whether you are relatively confident that the race can be called}" +
                "FirstPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "SecondPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "ThirdPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "FourthPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "Analysis: {Use this area to perform any step by step thinking and analysis you need to do}" +
                "Confidence: {A relative idea of confidence in prediction }" +
                $"Focus solely on the relative ability and potential of the horses, without speculating on odds. {Environment.NewLine}" +
                $"Focus more on providing a balanced analysis of the pros and cons, strengths and weaknesses of each horse based on the data, rather than definitive recommendations {Environment.NewLine}Race Data: {Environment.NewLine}```{Environment.NewLine}" + text + $"{Environment.NewLine}```{Environment.NewLine}{Environment.NewLine}Answer:{Environment.NewLine}";

            return prompt;
        }

        public static string BuildProsAndConsPrompt(string raceText)
        {
            return
                $"Answer as a racing statistician and expert. Be deeply analytical, consider all data in an authoritative, analytical style in painstaking detail. Based on the data and data only, you are to consider these horses and list pros and cons, with statistical data included, the trainers and jockeys, and the horse, on their ability to win or place the race. The rules are 1) Only use the data provided, never make up data to use. 2) Don't waste too much time on horses unlikely to win, focus on those that have a chance.3) However it is important to consider all horses, not just the first few, all horses 4) Do Not give a prediction, just the pros and cons. The race data is as follow: " +
                raceText + Environment.NewLine + "Repeating the rules: The rules are 1) Only use the data provided, never make up data to use. 2) Don't waste time on horses unlikely to win, focus on those that have a chance.3) It is important to consider all horses, not just the first few, all horses 4) Do Not give a prediction, just the pros and cons";
        }

        public static string BuildProsAndConsBasedPrompt(string raceText, string prosAndCons)
        {
            return
                $"Answer as a racing statistician and expert. You are going to be supplied some race data and a list of pros and cons about some of these horses. It is your task to predict with the highest probability the outcome of the race 1) Only use the data provided, never make up data to use. 2) Don't waste too much time on horses unlikely to win, focus on those that have a chance.3) However, it is important to consider all horses, not just the first few, all horses" +
                " The race data is as follow: " + Environment.NewLine + raceText + Environment.NewLine +
                "Pros and Cons: " + Environment.NewLine + prosAndCons + Environment.NewLine +
                $"It is important that you ALWAYS Format your answer in json with the following fields: {Environment.NewLine}" +
                "CanChoose:{bool - yes or no whether you are over 80% sure that the race can be called}" +
                "FirstPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "SecondPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "ThirdPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "FourthPlace: {ONLY PUT HORSE NUMBER AND NAME}" +
                "Repeating the rules 1) Only use the data provided, never make up data to use. 2) Don't waste time on horses unlikely to win, focus on those that have a chance.3) It is important to consider all horses, not just the first few, all horses" +
                "Answer:";

        }
    }
}
