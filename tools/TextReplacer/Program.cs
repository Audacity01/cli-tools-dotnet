using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextReplacer
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                PrintUsage();
                return 1;
            }

            string command = args[0].ToLower();

            switch (command)
            {
                case "replace":
                    if (args.Length < 4) { Console.Error.WriteLine("Need: file, search, replacement"); return 1; }
                    return ReplaceInFile(args[1], args[2], args[3], false);
                case "regex":
                    if (args.Length < 4) { Console.Error.WriteLine("Need: file, pattern, replacement"); return 1; }
                    return ReplaceInFile(args[1], args[2], args[3], true);
                case "bulk":
                    if (args.Length < 4) { Console.Error.WriteLine("Need: directory, search, replacement"); return 1; }
                    string pattern = args.Length > 4 ? args[4] : "*.*";
                    return BulkReplace(args[1], args[2], args[3], pattern);
                case "preview":
                    if (args.Length < 3) { Console.Error.WriteLine("Need: file, search"); return 1; }
                    return PreviewMatches(args[1], args[2]);
                default:
                    PrintUsage();
                    return 1;
            }
        }

        static int ReplaceInFile(string path, string search, string replacement, bool useRegex)
        {
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File not found: {path}");
                return 1;
            }

            string content = File.ReadAllText(path);
            string result;
            int count;

            if (useRegex)
            {
                var regex = new Regex(search);
                count = regex.Matches(content).Count;
                result = regex.Replace(content, replacement);
            }
            else
            {
                count = CountOccurrences(content, search);
                result = content.Replace(search, replacement);
            }

            if (count == 0)
            {
                Console.WriteLine("No matches found.");
                return 0;
            }

            File.WriteAllText(path, result);
            Console.WriteLine($"Replaced {count} occurrence(s) in {Path.GetFileName(path)}");
            return 0;
        }

        static int BulkReplace(string directory, string search, string replacement, string filePattern)
        {
            if (!Directory.Exists(directory))
            {
                Console.Error.WriteLine($"Directory not found: {directory}");
                return 1;
            }

            var files = Directory.GetFiles(directory, filePattern, SearchOption.AllDirectories);
            int totalFiles = 0, totalReplacements = 0;

            foreach (var file in files)
            {
                try
                {
                    string content = File.ReadAllText(file);
                    int count = CountOccurrences(content, search);
                    if (count > 0)
                    {
                        File.WriteAllText(file, content.Replace(search, replacement));
                        totalFiles++;
                        totalReplacements += count;
                        Console.WriteLine($"  {file} ({count} replacements)");
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"  Skipped {file}: {ex.Message}");
                }
            }

            Console.WriteLine($"\nDone: {totalReplacements} replacements across {totalFiles} file(s)");
            return 0;
        }

        static int PreviewMatches(string path, string search)
        {
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File not found: {path}");
                return 1;
            }

            var lines = File.ReadAllLines(path);
            int matchCount = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(search))
                {
                    matchCount++;
                    Console.WriteLine($"  Line {i + 1}: {lines[i].Trim()}");
                }
            }

            Console.WriteLine($"\n{matchCount} line(s) contain \"{search}\"");
            return 0;
        }

        static int CountOccurrences(string text, string search)
        {
            int count = 0, index = 0;
            while ((index = text.IndexOf(search, index, StringComparison.Ordinal)) != -1)
            {
                count++;
                index += search.Length;
            }
            return count;
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: TextReplacer <command> [args]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  replace <file> <search> <replace>         Find and replace in file");
            Console.WriteLine("  regex <file> <pattern> <replace>          Regex find and replace");
            Console.WriteLine("  bulk <dir> <search> <replace> [pattern]   Replace across all files");
            Console.WriteLine("  preview <file> <search>                   Preview matches without changing");
        }
    }
}
