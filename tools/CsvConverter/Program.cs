using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CsvConverter
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                PrintUsage();
                return 1;
            }

            string command = args[0].ToLower();
            string inputFile = args[1];

            if (!File.Exists(inputFile))
            {
                Console.Error.WriteLine($"File not found: {inputFile}");
                return 1;
            }

            string outputFile = args.Length > 2 ? args[2] : null;

            switch (command)
            {
                case "to-json":
                    return CsvToJson(inputFile, outputFile);
                case "to-csv":
                    return JsonToCsv(inputFile, outputFile);
                case "stats":
                    return ShowStats(inputFile);
                default:
                    Console.Error.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    return 1;
            }
        }

        static int CsvToJson(string csvPath, string outputPath)
        {
            try
            {
                var lines = File.ReadAllLines(csvPath);
                if (lines.Length == 0) { Console.WriteLine("[]"); return 0; }

                var headers = ParseCsvLine(lines[0]);
                var records = new List<Dictionary<string, string>>();

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    var values = ParseCsvLine(lines[i]);
                    var record = new Dictionary<string, string>();
                    for (int j = 0; j < headers.Length && j < values.Length; j++)
                    {
                        record[headers[j]] = values[j];
                    }
                    records.Add(record);
                }

                var json = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });

                if (outputPath != null)
                {
                    File.WriteAllText(outputPath, json);
                    Console.WriteLine($"Written to {outputPath}");
                }
                else
                {
                    Console.WriteLine(json);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static int JsonToCsv(string jsonPath, string outputPath)
        {
            try
            {
                var json = File.ReadAllText(jsonPath);
                var records = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json);

                if (records == null || records.Count == 0)
                {
                    Console.WriteLine("No data");
                    return 0;
                }

                var headers = records.SelectMany(r => r.Keys).Distinct().ToList();
                var sb = new StringBuilder();
                sb.AppendLine(string.Join(",", headers));

                foreach (var record in records)
                {
                    var values = headers.Select(h =>
                    {
                        if (record.TryGetValue(h, out var val))
                        {
                            var str = val.ToString();
                            if (str.Contains(",") || str.Contains("\""))
                                return $"\"{str.Replace("\"", "\"\"")}\"";
                            return str;
                        }
                        return "";
                    });
                    sb.AppendLine(string.Join(",", values));
                }

                var result = sb.ToString().TrimEnd();
                if (outputPath != null)
                {
                    File.WriteAllText(outputPath, result);
                    Console.WriteLine($"Written to {outputPath}");
                }
                else
                {
                    Console.WriteLine(result);
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static int ShowStats(string csvPath)
        {
            var lines = File.ReadAllLines(csvPath);
            if (lines.Length == 0)
            {
                Console.WriteLine("Empty file");
                return 0;
            }

            var headers = ParseCsvLine(lines[0]);
            int dataRows = lines.Skip(1).Count(l => !string.IsNullOrWhiteSpace(l));

            Console.WriteLine($"File: {Path.GetFileName(csvPath)}");
            Console.WriteLine($"Columns: {headers.Length}");
            Console.WriteLine($"Data rows: {dataRows}");
            Console.WriteLine($"Headers: {string.Join(", ", headers)}");

            return 0;
        }

        static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            var current = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(current.ToString().Trim());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
            fields.Add(current.ToString().Trim());
            return fields.ToArray();
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: CsvConverter <command> <input-file> [output-file]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  to-json   Convert CSV to JSON");
            Console.WriteLine("  to-csv    Convert JSON to CSV");
            Console.WriteLine("  stats     Show CSV file statistics");
        }
    }
}
