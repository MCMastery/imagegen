using System;
using System.IO;

namespace ImageGen
{
    class ImageGen
    {
        static void Main(string[] args)
        {
            Execution.LoadColorCodes();
            Console.WriteLine("[ImageGen v0.0.1]");
            Console.WriteLine("Type 'exit' to exit the program.");
            Console.WriteLine();

            // open file
            if (args.Length == 1 && (args[0].EndsWith(".ig") || args[0].EndsWith(".txt")))
            {
                Console.WriteLine("Executing " + args[0]);
                Console.WriteLine();
                string code = "";
                foreach (string line in File.ReadLines(args[0]))
                    code += line + " ";
                Console.WriteLine("> " + code);
                Execution.Execute(code);
                Console.WriteLine();
            }

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input == "exit")
                    break;
                Execution.Execute(input);
                Console.WriteLine();
            }
        }
    }
}
