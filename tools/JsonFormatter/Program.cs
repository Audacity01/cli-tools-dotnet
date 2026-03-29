using System;
using System.IO;
using System.Text.Json;

namespace JsonFormatter
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
            string filePath = args[1];

            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"File not found: {filePath}");
                return 1;
            }

            string json = File.ReadAllText(filePath);

            switch (command)
            {
                case "format":
                    return FormatJson(json);
                case "minify":
                    return MinifyJson(json);
                case "validate":
                    return ValidateJson(json);
                default:
                    Console.Error.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    return 1;
            }
        }

        static int FormatJson(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string formatted = JsonSerializer.Serialize(doc.RootElement, options);
                Console.WriteLine(formatted);
                return 0;
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Invalid JSON: {ex.Message}");
                return 1;
            }
        }

        static int MinifyJson(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions { WriteIndented = false };
                string minified = JsonSerializer.Serialize(doc.RootElement, options);
                Console.WriteLine(minified);
                return 0;
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Invalid JSON: {ex.Message}");
                return 1;
            }
        }

        static int ValidateJson(string json)
        {
            try
            {
                JsonDocument.Parse(json);
                Console.WriteLine("Valid JSON.");
                return 0;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Invalid JSON: {ex.Message}");
                return 1;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: JsonFormatter <command> <file>");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  format    Pretty-print JSON");
            Console.WriteLine("  minify    Minify JSON");
            Console.WriteLine("  validate  Check if JSON is valid");
        }
    }
}
