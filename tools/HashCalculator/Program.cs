using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace HashCalculator
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                PrintUsage();
                return 1;
            }

            string command = args[0].ToLower();

            switch (command)
            {
                case "file":
                    if (args.Length < 2) { Console.Error.WriteLine("Missing file path"); return 1; }
                    return HashFile(args[1], args.Length > 2 ? args[2] : "sha256");
                case "text":
                    if (args.Length < 2) { Console.Error.WriteLine("Missing text"); return 1; }
                    return HashText(args[1], args.Length > 2 ? args[2] : "sha256");
                case "verify":
                    if (args.Length < 3) { Console.Error.WriteLine("Missing file path or expected hash"); return 1; }
                    return VerifyFile(args[1], args[2], args.Length > 3 ? args[3] : "sha256");
                case "compare":
                    if (args.Length < 3) { Console.Error.WriteLine("Need two file paths"); return 1; }
                    return CompareFiles(args[1], args[2]);
                default:
                    Console.Error.WriteLine($"Unknown command: {command}");
                    PrintUsage();
                    return 1;
            }
        }

        static int HashFile(string path, string algo)
        {
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File not found: {path}");
                return 1;
            }

            try
            {
                using var hasher = CreateHashAlgorithm(algo);
                using var stream = File.OpenRead(path);
                var hash = hasher.ComputeHash(stream);
                var hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                Console.WriteLine($"{algo.ToUpper()}: {hex}");
                Console.WriteLine($"File: {Path.GetFileName(path)}");
                Console.WriteLine($"Size: {new FileInfo(path).Length} bytes");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        static int HashText(string text, string algo)
        {
            using var hasher = CreateHashAlgorithm(algo);
            var bytes = Encoding.UTF8.GetBytes(text);
            var hash = hasher.ComputeHash(bytes);
            var hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            Console.WriteLine($"{algo.ToUpper()}: {hex}");
            return 0;
        }

        static int VerifyFile(string path, string expectedHash, string algo)
        {
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"File not found: {path}");
                return 1;
            }

            using var hasher = CreateHashAlgorithm(algo);
            using var stream = File.OpenRead(path);
            var hash = hasher.ComputeHash(stream);
            var actual = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            if (actual.Equals(expectedHash.ToLowerInvariant()))
            {
                Console.WriteLine("MATCH — file integrity verified");
                return 0;
            }
            else
            {
                Console.WriteLine("MISMATCH — file may be corrupted");
                Console.WriteLine($"Expected: {expectedHash.ToLowerInvariant()}");
                Console.WriteLine($"Actual:   {actual}");
                return 1;
            }
        }

        static int CompareFiles(string path1, string path2)
        {
            if (!File.Exists(path1) || !File.Exists(path2))
            {
                Console.Error.WriteLine("One or both files not found");
                return 1;
            }

            using var hasher = SHA256.Create();

            using var s1 = File.OpenRead(path1);
            var h1 = BitConverter.ToString(hasher.ComputeHash(s1)).Replace("-", "").ToLowerInvariant();

            hasher.Initialize();
            using var s2 = File.OpenRead(path2);
            var h2 = BitConverter.ToString(hasher.ComputeHash(s2)).Replace("-", "").ToLowerInvariant();

            if (h1 == h2)
                Console.WriteLine("Files are identical");
            else
                Console.WriteLine("Files are different");

            Console.WriteLine($"  {Path.GetFileName(path1)}: {h1}");
            Console.WriteLine($"  {Path.GetFileName(path2)}: {h2}");
            return h1 == h2 ? 0 : 1;
        }

        static HashAlgorithm CreateHashAlgorithm(string name)
        {
            switch (name.ToLower())
            {
                case "md5": return MD5.Create();
                case "sha1": return SHA1.Create();
                case "sha256": return SHA256.Create();
                case "sha384": return SHA384.Create();
                case "sha512": return SHA512.Create();
                default: throw new ArgumentException($"Unsupported algorithm: {name}");
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: HashCalculator <command> [args]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  file <path> [algo]              Hash a file (default: sha256)");
            Console.WriteLine("  text <string> [algo]            Hash a string");
            Console.WriteLine("  verify <path> <hash> [algo]     Verify file against expected hash");
            Console.WriteLine("  compare <file1> <file2>         Compare two files by hash");
            Console.WriteLine();
            Console.WriteLine("Algorithms: md5, sha1, sha256, sha384, sha512");
        }
    }
}
