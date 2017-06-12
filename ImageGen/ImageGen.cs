using System;

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
