using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Text;
using GPTNet;
using GPTNet.Models;
using Microsoft.Extensions.Configuration;
using TambleBot.PromptBuilder;

namespace TambleBot
{
    public class CSVBot : Bot
    {
        public async Task<Results> Predict()
        {
            Results results = new Results();

            var file = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.csv").FirstOrDefault();

            if (file == null) return results;

            string text = string.Join(Environment.NewLine, TrimDataLines(File.ReadAllLines(file.FullName)));

            string prompt = RaceDataPrompts.BuildPrompt(text);

            GPTChat bot = GetBot();//reset each time, currently no way to reset conversation on ChatBot

            GPTResponse response = await bot.Chat(prompt);

            if (response.IsError)
            {
                results.ErrorsList.Add(new Error
                {
                    Race = file.Name,
                    Response = response.Error
                });
            }
            else
            {
                results.ResultsList.Add(new Result
                {
                    Race = file.Name,
                    Response = response.Response
                });
            }

            return results;
        }

        public IEnumerable<string> TrimDataLines(string[] lines)
        {
            foreach (var line in lines)
            {
                string[] dataArray = line.Split(',');

                string values = string.Join(",", dataArray.Take(124));

                yield return values;
            }
        }


    }
}
