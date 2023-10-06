using GPTNet.Models;
using GPTNet;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TambleBot
{
    public abstract class Bot
    {
        public GPTChat GetBot(string model = null)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            model = model ?? configuration["Model"];

            //Uses GPTApiProperties object, the correct new way to do things
            GPTApiProperties properties = GPTApiProperties.Create<GPTApiAnthropic>(
                configuration["ApiKey"], model, configuration["Version"], httpClient: new HttpClient{ Timeout = TimeSpan.FromSeconds(600) });
            properties.Temperature = 0.1m;

            return new GPTChat(properties);
        }

        protected async Task<Results> AskBot(string prompt, string model = null)
        {
            Results results = new Results();

            GPTChat bot = GetBot(model);//reset each time, currently no way to reset conversation on ChatBot

            GPTResponse response = await bot.Chat(prompt);

            if (response.IsError)
            {
                results.ErrorsList.Add(new Error
                {
                    Response = response.Error
                });
            }
            else
            {
                results.ResultsList.Add(new Result
                {
                    Response = response.Response
                });
            }

            return results;
        }
    }
}
