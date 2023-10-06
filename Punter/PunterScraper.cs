using AngleSharp.Dom;
using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using System.Net.Http;
using Microsoft.VisualBasic.CompilerServices;
using TambleBot.Punter.Jockey;
using TambleBot.Punter.Trainer;
using System.Net.Sockets;
using GPTNet;
using TambleBot.Punter.Horse;

namespace TambleBot.Punter
{
    public enum LoadSettings { None, Minimal, Full }

    public class PunterScraper
    {
        private LoadSettings _loadJockeys = LoadSettings.Minimal;
        private LoadSettings _loadTrainers = LoadSettings.Minimal;
        private LoadSettings _loadHorses = LoadSettings.Full;

        public async Task<RaceData> FetchAndParseHTMLAsync(
            string url, 
            LoadSettings jockeyData = LoadSettings.Minimal, 
            LoadSettings trainerData = LoadSettings.Minimal, 
            LoadSettings horseData = LoadSettings.Full)
        {
            Log("Parsing Race Page");

            _loadJockeys = jockeyData;
            _loadTrainers = trainerData;
            _loadHorses = horseData;

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);
            var tables = document.All.Where(t =>
                t.ClassName?.Equals("form-guide-overview__table unresulted") ?? false).ToList();

            Log("Getting race data");
            string data = await GetSpreadsheetData(document);
            Log("Processing race data");
            string[] dataLines = data.Split(new char[] { '\r', '\n' });

            string[] processedData = TrimDataLines(dataLines).ToArray();

            string trackCondition = GetTrackCondition(document);
            StringBuilder jockeyDescriptions = new StringBuilder();
            StringBuilder trainerDescriptions = new StringBuilder();
            StringBuilder horseDescriptions = new StringBuilder();

            if (tables.Count == 0)
            {
                Console.WriteLine("No overview tables found .. race may already be resulted");
                return null;
            }
            else if (tables.Count > 1)
            {
                Console.WriteLine("More than one overview table found. Using first ...");
            }

            if (!await GetHorseDescriptions(dataLines, horseDescriptions)) return null;

            if (_loadJockeys != LoadSettings.None)
            {
                if (!await GetJockeyDescriptions(dataLines, jockeyDescriptions)) return null;
            }

            if (_loadTrainers != LoadSettings.None)
            {
                if (!await GetTrainerDescriptions(dataLines, trainerDescriptions)) return null;
            }

            return new RaceData(trackCondition, processedData, jockeyDescriptions, trainerDescriptions, horseDescriptions);
        }

        private string GetTrackCondition(IDocument document)
        {
            return document.QuerySelectorAll("span")?.Where(x => x.ClassName?.Equals("form-header__track-condition") ?? false).FirstOrDefault().TextContent.Trim()??"Unknown";
        }

        private async Task<bool> GetHorseDescriptions(string[] dataLines, StringBuilder horseDescriptions)
        {
            if (_loadHorses == LoadSettings.Full)
            {
                Log("Getting Horse locations");
                Dictionary<string, string> horseUrls = GetHorseUrls(dataLines);
                if (horseUrls == null)
                {
                    return false;
                }

                Log("Getting Horse data");
                var horseData = await GetHorseData(horseUrls);

                foreach (var punterHorse in horseData)
                {
                    horseDescriptions.AppendLine($"Horse: {punterHorse.Description}");
                    foreach (var stat in punterHorse.Stats)
                    {
                        horseDescriptions.AppendLine($"{stat}");
                    }
                }
            }

            return true;
        }

        private async Task<bool> GetJockeyDescriptions(string[] dataLines, StringBuilder jockeyDescriptions)
        {
            if (_loadJockeys != LoadSettings.None)
            {
                Log("Getting Jockey locations");
                Dictionary<string, string> jockeyUrls = GetJockeyUrls(dataLines);
                if (jockeyUrls == null)
                {
                    return false;
                }

                Log("Getting Jockey data");
                var jockeyData = await GetJockeyData(jockeyUrls);

                foreach (var punterJockey in jockeyData)
                {
                    jockeyDescriptions.AppendLine($"Jockey: {punterJockey.Description}");
                    foreach (var stat in punterJockey.Stats)
                    {
                        jockeyDescriptions.AppendLine($"{stat}");
                    }
                }
            }

            return true;
        }

        private async Task<bool> GetTrainerDescriptions(string[] dataLines, StringBuilder trainerDescriptions)
        {
            if (_loadTrainers != LoadSettings.None)
            {
                Log("Getting Trainer locations");
                Dictionary<string, string> trainerUrls = GetTrainerUrls(dataLines);
                if (trainerUrls == null)
                {
                    return false;
                }

                Log("Getting Trainer data");
                var trainerData = await GetTrainerData(trainerUrls);

                foreach (var punterTrainer in trainerData)
                {
                    trainerDescriptions.AppendLine($"Trainer: {punterTrainer.Description}");
                    foreach (var stat in punterTrainer.Stats)
                    {
                        trainerDescriptions.AppendLine($"{stat}");
                    }
                }
            }

            return true;
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

        private Dictionary<string, string> GetJockeyUrls(string[] dataLines)
        {
            Dictionary<string, string> jockeyUrls = new Dictionary<string, string>();
            string headerLine = dataLines[0];

            int jockeyIndex = headerLine.Split(new char[] { ',' }).ToList().IndexOf("Jockey");
            if (jockeyIndex == -1)
            {
                Console.WriteLine("No jockey found");
                return null;
            }

            int jockeyUrlIndex = headerLine.Split(new char[] { ',' }).ToList().IndexOf("Jockey Profile Url");
            if (jockeyUrlIndex == -1)
            {
                Console.WriteLine("No jockey profile urls found");
                return null;
            }

            foreach (var line in dataLines.Skip(1))
            {
                if (line == null || line.Trim().Length == 0) continue;

                string[] horseDetails = line.Split(new char[] { ',' });

                jockeyUrls[horseDetails[jockeyIndex]] = horseDetails[jockeyUrlIndex];
            }

            return jockeyUrls;
        }

        private Dictionary<string, string> GetTrainerUrls(string[] dataLines)
        {
            Dictionary<string, string> trainerUrls = new Dictionary<string, string>();
            string headerLine = dataLines[0];

            int trainerIndex = headerLine.Split(new char[] { ',' }).ToList().IndexOf("Trainer");
            if (trainerIndex == -1)
            {
                Console.WriteLine("No trainer found");
                return null;
            }

            int trainerUrlIndex = headerLine.Split(new char[] { ',' }).ToList().IndexOf("Trainer Profile Url");
            if (trainerUrlIndex == -1)
            {
                Console.WriteLine("No trainer profile urls found");
                return null;
            }

            foreach (var line in dataLines.Skip(1))
            {
                if (line == null || line.Trim().Length == 0) continue;

                string[] horseDetails = line.Split(new char[] { ',' });

                trainerUrls[horseDetails[trainerIndex]] = horseDetails[trainerUrlIndex];
            }

            return trainerUrls;
        }

        private Dictionary<string, string> GetHorseUrls(string[] dataLines)
        {
            Dictionary<string, string> horseUrls = new Dictionary<string, string>();
            string headerLine = dataLines[0];

            int horseIndex = headerLine.Split(new char[] { ',' }).ToList().IndexOf("Horse Name");
            if (horseIndex == -1)
            {
                Console.WriteLine("No horse found");
                return null;
            }

            int horseUrlIndex = headerLine.Split(new char[] { ',' }).ToList().IndexOf("Horse Profile Url");
            if (horseUrlIndex == -1)
            {
                Console.WriteLine("No horse profile urls found");
                return null;
            }

            foreach (var line in dataLines.Skip(1))
            {
                if (line == null || line.Trim().Length == 0) continue;

                string[] horseDetails = line.Split(new char[] { ',' });

                horseUrls[horseDetails[horseIndex]] = horseDetails[horseUrlIndex];
            }

            return horseUrls;
        }

        private async Task<string> GetSpreadsheetData(IDocument document)
        {
            var anchorElement = document.QuerySelector("a[data-analytics='Form Guide : Download : Spreadsheet']");
            string href = anchorElement.GetAttribute("href");

            return await DownloadCsvFromUrl($"https://www.punters.com.au{href}");
        }

        private async Task<List<PunterJockey>> GetJockeyData(Dictionary<string, string> urls)
        {
            List<PunterJockey> jockeys = new List<PunterJockey>();

            Log("Parsing jockey urls");
            foreach (var url in urls)
            {
                jockeys.Add(await GetJockey(url.Key, url.Value));
            }

            return jockeys;
        }

        private async Task<PunterJockey> GetJockey(string name, string url)
        {
            PunterJockey jockey = new PunterJockey();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            Log("Loading Jockey data");
            var content = document.QuerySelector("div[class='profile-bio']");


            var element = document.QuerySelector("a[data-type='isBlackbook']");
            if (element != null)
            {
                jockey.Description = element.GetAttribute("data-name");
            }
            else
            {
                jockey.Description = content?.TextContent.Trim();
            }

            Log($"Jockey: {jockey.Description}");
            var tables = document.QuerySelectorAll("table");

            Log("Getting stats");
            ;
            foreach (var table in tables)
            {
                PunterJockeyStats stats = new PunterJockeyStats();

                var thElements = table.QuerySelectorAll("th");

                List<int> ignoreColumns = new List<int>();

                if (!ExtractJockeyData(thElements)) continue;

                for (int i = 0; i < thElements.Length; i++)
                {
                    string header = thElements[i].TextContent.Trim();

                    //if (header == "Avg Win Odds" || header == "ROI")
                    //{
                    //    ignoreColumns.Add(i);
                    //}
                    //else
                    //{
                        stats.Headers.Add(thElements[i].TextContent.Trim());
                    //}
                }

                var body = table.QuerySelectorAll("tbody");

                var rows = body[0].QuerySelectorAll("tr");

                foreach (var row in rows)
                {
                    List<string> rowData = new List<string>();
                    var data = row.QuerySelectorAll("td");
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (ignoreColumns.Contains(i)) continue;

                        rowData.Add(data[i].TextContent.Trim());
                    }

                    stats.Data.Add(rowData);
                }

                jockey.Stats.Add(stats);
            }

            return jockey;
        }

        private bool ExtractJockeyData(IHtmlCollection<IElement> thElements)
        {
            return 
                thElements.Length > 0 &&

                (_loadJockeys == LoadSettings.Full && !thElements[0].TextContent.StartsWith("Prize Money") ||
                _loadJockeys == LoadSettings.Minimal &&
                (thElements[0].TextContent.StartsWith("Overall Stats") ||
                 thElements[0].TextContent.StartsWith("Track")));
        }

        private async Task<List<PunterTrainer>> GetTrainerData(Dictionary<string, string> urls)
        {
            List<PunterTrainer> trainers = new List<PunterTrainer>();

            Log("Parsing trainer urls");
            foreach (var url in urls)
            {
                trainers.Add(await GetTrainer(url.Key.Replace("\"", ""), url.Value));
            }

            return trainers;
        }

        private async Task<List<PunterHorse>> GetHorseData(Dictionary<string, string> urls)
        {
            List<PunterHorse> horses = new List<PunterHorse>();

            Log("Parsing horse urls");
            foreach (var url in urls)
            {
                horses.Add(await GetHorse(url.Key, url.Value));
            }

            return horses;
        }

        private async Task<PunterTrainer> GetTrainer(string name, string url)
        {
            PunterTrainer trainer = new PunterTrainer();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);
            Log("Loading Trainer data");
            //var content = document.QuerySelector("div[class='profile-bio']");

            //trainer.Description = content?.TextContent.Trim();
            trainer.Description = name;
            Log($"Trainer: {trainer.Description}");
            var tables = document.QuerySelectorAll("table");

            Log("Getting stats");
            foreach (var table in tables)
            {
                PunterTrainerStats stats = new PunterTrainerStats();

                var thElements = table.QuerySelectorAll("th");

                if (!ExtractTrainerData(thElements)) continue;

                foreach (var thElement in thElements)
                {
                    stats.Headers.Add(thElement.TextContent.Trim());
                }

                var body = table.QuerySelectorAll("tbody");

                var rows = body[0].QuerySelectorAll("tr");

                foreach (var row in rows)
                {
                    List<string> rowData = new List<string>();
                    var data = row.QuerySelectorAll("td");
                    foreach (var item in data)
                    {
                        rowData.Add(item.TextContent.Trim());
                    }

                    stats.Data.Add(rowData);
                }

                trainer.Stats.Add(stats);
            }

            return trainer;
        }

        private bool ExtractTrainerData(IHtmlCollection<IElement> thElements)
        {
            return
                thElements.Length > 0 &&
                (_loadTrainers == LoadSettings.Full && !thElements[0].TextContent.StartsWith("Prize Money") ||
                 
                 _loadTrainers == LoadSettings.Minimal && 
                    (thElements[0].TextContent.StartsWith("Overall Stats") ||
                     thElements[0].TextContent.StartsWith("Track"))
                 
                 );
        }

        private async Task<PunterHorse> GetHorse(string name, string url)
        {
            PunterHorse horse = new PunterHorse();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url + "Stats");
            Log("Loading Horse data");
            var content = document.QuerySelector("div[class='profile-bio']");

            horse.Description = name;
            Log($"Horse: {horse.Description}");
            var tables = document.QuerySelectorAll("table");

            Log("Getting stats");
            foreach (var table in tables)
            {
                PunterHorseStats stats = new PunterHorseStats();

                var thElements = table.QuerySelectorAll("th");

                foreach (var thElement in thElements)
                {
                    stats.Headers.Add(thElement.TextContent.Trim());
                }

                var body = table.QuerySelectorAll("tbody");

                var rows = body[0].QuerySelectorAll("tr");

                foreach (var row in rows)
                {
                    List<string> rowData = new List<string>();
                    var data = row.QuerySelectorAll("td");
                    foreach (var item in data)
                    {
                        rowData.Add(item.TextContent.Trim());
                    }

                    stats.Data.Add(rowData);
                }

                horse.Stats.Add(stats);
            }

            return horse;
        }

        public IEnumerable<string> TrimDataLines(string[] lines)
        {
            foreach (var line in lines)
            {
                string[] dataArray = line.Split(',');

                string values = string.Join(",", dataArray.Skip(1).Take(123));

                yield return values;
            }
        }

        public async Task<string> DownloadCsvFromUrl(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
