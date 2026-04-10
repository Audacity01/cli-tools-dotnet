using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace UrlEncoder
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
            string input = string.Join(" ", args.Skip(1));

            switch (command)
            {
                case "encode":
                    Console.WriteLine(Uri.EscapeDataString(input));
                    return 0;
                case "decode":
                    Console.WriteLine(Uri.UnescapeDataString(input));
                    return 0;
                case "parse":
                    return ParseUrl(input);
                case "build":
                    return BuildUrl(args.Skip(1).ToArray());
                default:
                    Console.Error.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    return 1;
            }
        }

        static int ParseUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                Console.WriteLine($"Scheme:   {uri.Scheme}");
                Console.WriteLine($"Host:     {uri.Host}");
                Console.WriteLine($"Port:     {uri.Port}");
                Console.WriteLine($"Path:     {uri.AbsolutePath}");
                Console.WriteLine($"Query:    {uri.Query}");
                Console.WriteLine($"Fragment: {uri.Fragment}");

                if (!string.IsNullOrEmpty(uri.Query))
                {
                    Console.WriteLine("\nQuery Parameters:");
                    var query = uri.Query.TrimStart('?');
                    foreach (var pair in query.Split('&'))
                    {
                        var parts = pair.Split('=', 2);
                        var key = Uri.UnescapeDataString(parts[0]);
                        var value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
                        Console.WriteLine($"  {key} = {value}");
                    }
                }
                return 0;
            }
            catch (UriFormatException ex)
            {
                Console.Error.WriteLine($"Invalid URL: {ex.Message}");
                return 1;
            }
        }

        static int BuildUrl(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Need at least a base URL");
                return 1;
            }

            string baseUrl = args[0];
            var queryParams = new List<string>();

            for (int i = 1; i < args.Length; i++)
            {
                var parts = args[i].Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = Uri.EscapeDataString(parts[0]);
                    var val = Uri.EscapeDataString(parts[1]);
                    queryParams.Add($"{key}={val}");
                }
            }

            var separator = baseUrl.Contains('?') ? '&' : '?';
            if (queryParams.Count > 0)
                baseUrl += separator + string.Join("&", queryParams);

            Console.WriteLine(baseUrl);
            return 0;
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: UrlEncoder <command> <input>");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  encode <text>              URL-encode a string");
            Console.WriteLine("  decode <text>              URL-decode a string");
            Console.WriteLine("  parse <url>                Break down a URL into components");
            Console.WriteLine("  build <base> [key=val...]  Build a URL with query parameters");
        }
    }
}
