using AngleSharp.Dom;
using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TambleBot
{
    public class TABScraper
    {
        private string _url = "https://www.tab.com.au/racing/meetings/today/R";

        public async Task<IDocument> FetchAndParseHTMLAsync()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(_url);
            return document;
        }
    }
}