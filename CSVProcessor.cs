using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace TambleBot
{
    public class CSVProcessor
    {
        public string ReadAndProcessCsv(string inputFilePath, List<int> columnsToRemove)
        {
            StringBuilder processedCsv = new StringBuilder();
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                using (CsvReader csvReader = new CsvReader(reader, new CsvConfiguration(cultureInfo) { HasHeaderRecord = false }))
                {
                    using (StringWriter writer = new StringWriter(processedCsv))
                    {
                        using (CsvWriter csvWriter = new CsvWriter(writer, new CsvConfiguration(cultureInfo) { HasHeaderRecord = false }))
                        {
                            while (csvReader.Read())
                            {
                                List<string> rowData = new List<string>();
                                for (int i = 0; csvReader.TryGetField<string>(i, out string value); i++)
                                {
                                    if (!columnsToRemove.Contains(i))
                                    {
                                        rowData.Add(value);
                                    }
                                }
                                csvWriter.WriteRecord(rowData);
                            }
                        }
                    }
                }
            }

            return processedCsv.ToString().Replace("\r\n", Environment.NewLine);
        }
    }
}
