using AngleSharp.Dom;
using AngleSharp;
using System;
using System.Threading.Tasks;

namespace TambleBot
{
    public class RaceNetScraper
    {
        public async Task<IDocument> FetchAndParseHTMLAsync(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);
            return document;
        }
    }
}
