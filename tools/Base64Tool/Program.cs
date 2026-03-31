using System;
using System.IO;
using System.Text;

namespace Base64Tool
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
            string input = args[1];

            switch (command)
            {
                case "encode":
                    return EncodeText(input);
                case "decode":
                    return DecodeText(input);
                case "encode-file":
                    return EncodeFile(input, args.Length > 2 ? args[2] : null);
                case "decode-file":
                    if (args.Length < 3) { Console.Error.WriteLine("Missing output path"); return 1; }
                    return DecodeFile(input, args[2]);
                case "check":
                    return CheckBase64(input);
                default:
                    Console.Error.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    return 1;
            }
        }

        static int EncodeText(string text)
        {
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
            Console.WriteLine(encoded);
            return 0;
        }

        static int DecodeText(string base64)
        {
            try
            {
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
                Console.WriteLine(decoded);
                return 0;
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("Invalid base64 string");
                return 1;
            }
        }

        static int EncodeFile(string filePath, string outputPath)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"File not found: {filePath}");
                return 1;
            }

            var bytes = File.ReadAllBytes(filePath);
            var encoded = Convert.ToBase64String(bytes);
            var size = bytes.Length;

            if (outputPath != null)
            {
                File.WriteAllText(outputPath, encoded);
                Console.WriteLine($"Encoded {size} bytes -> {outputPath}");
            }
            else
            {
                Console.WriteLine(encoded);
            }
            return 0;
        }

        static int DecodeFile(string base64Path, string outputPath)
        {
            if (!File.Exists(base64Path))
            {
                Console.Error.WriteLine($"File not found: {base64Path}");
                return 1;
            }

            try
            {
                var base64 = File.ReadAllText(base64Path).Trim();
                var bytes = Convert.FromBase64String(base64);
                File.WriteAllBytes(outputPath, bytes);
                Console.WriteLine($"Decoded {bytes.Length} bytes -> {outputPath}");
                return 0;
            }
            catch (FormatException)
            {
                Console.Error.WriteLine("File does not contain valid base64");
                return 1;
            }
        }

        static int CheckBase64(string input)
        {
            try
            {
                Convert.FromBase64String(input);
                var bytes = Convert.FromBase64String(input);
                Console.WriteLine("Valid base64");
                Console.WriteLine($"Decoded size: {bytes.Length} bytes");
                return 0;
            }
            catch
            {
                Console.WriteLine("Not valid base64");
                return 1;
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: Base64Tool <command> <input> [output]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  encode <text>                  Encode text to base64");
            Console.WriteLine("  decode <base64>                Decode base64 to text");
            Console.WriteLine("  encode-file <path> [output]    Encode a file to base64");
            Console.WriteLine("  decode-file <path> <output>    Decode base64 file to binary");
            Console.WriteLine("  check <string>                 Validate if string is base64");
        }
    }
}
