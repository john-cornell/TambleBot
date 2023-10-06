using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GPTNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TambleBot.PromptBuilder;

namespace TambleBot.Punter.Predictors
{
    public class ExpertsPunterPredictor : PunterPredictor
    {
        public override async Task<string> Predict(RaceData data, string model)
        {
            string raceText = GetRaceDataText(data);

            string prompt =
                RaceDataPrompts.BuildExpertsDescriptionsPrompt(raceText);

            Log("Getting experts");
            var response = await GetBot().Chat(prompt);

            if (response.IsError)
            {
                Console.WriteLine(response.Error);
                return "Can't predict";
            }

            Console.WriteLine("Experts Breakdown: ");
            Console.WriteLine(response.Response);

            try
            {
                var json = ParseJson(response.Response);

                Console.WriteLine(json.ToString(Formatting.Indented));

                var experts = json.GetValue("Experts");
                List<string> methods = new List<string>();

                Log("Getting opinions");
                int index = 1;
                foreach (var opinion in experts.Values<string>("Opinion"))
                {
                    Console.WriteLine(opinion);
                    methods.Add(opinion);
                }

                List<string> 
                    responses = new List<string>();

                foreach (var method in methods)
                {
                    Log((index++).ToString());
                    string methodPrompt = RaceDataPrompts.BuildExpertPrompt(raceText, method);
                    GPTResponse methodResponse = await GetBot().Chat(methodPrompt);

                    if (methodResponse.IsError)
                    {
                        Console.WriteLine(methodResponse.Error);
                    }
                    else
                    {
                        Console.WriteLine(methodResponse.Response);
                        responses.Add(methodResponse.Response);
                    }
                }

                string adjudicatorPrompt = RaceDataPrompts.BuildAdjudicatorPrompt(raceText, methods, responses);
                Log("Predicting ...");
                var prediction = await GetBot().Chat(adjudicatorPrompt);

                if (response.IsError)
                {
                    Console.WriteLine(response.Error);
                    return "Can't predict";
                }

                return prediction.Response;
            }
            catch(Exception e)
            {
                Console.WriteLine("Looks like bot didn't return valid Json");
                Console.WriteLine(e.Message);
                return "Can't predict";
            }
        }

        public JObject ParseJson(string input)
        {
            // Define the Regular Expression to find a JSON object
            Regex regex = new Regex(
                @"(?<json>{(?:[^{}]|(?<Nested>{)|(?<-Nested>}))*(?(Nested)(?!))})",
                RegexOptions.Multiline
            );

            // Run the regular expression on the input string
            Match match = regex.Match(input);

            JObject json;
            if (match.Success)
            {
                // Now get our JSON string
                string jsonString = match.Groups["json"].Value;

                // Try to parse the JSON string
                try
                {
                    json = JObject.Parse(jsonString);
                }
                catch (JsonReaderException)
                {
                    json = new JObject();
                }
            }
            else
            {
                json = new JObject();
            }

            return json;
        }

}

}
