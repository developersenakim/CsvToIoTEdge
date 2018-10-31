using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace CsvToIoTEdge
{
    class LogBuilder
    {
        static public void DrawProgressBar(int progress_current, int progress_max)
        {
            Console.CursorVisible = false;
            int temptop = Console.CursorTop + 1;
            Console.SetCursorPosition(1, temptop);
            for (int i = 0; i < progress_max + 1; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    Console.Write("■");
                }
                Console.Write(i + "/" + progress_max);
                if (i == progress_max)
                {
                    Console.Write("\n\n");
                }
                else
                {
                    Console.SetCursorPosition(1, temptop);
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
        static public void WriteErrorMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        static public void WriteWarningMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        static public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }
        static public void QuitMessage()
        {
            bool confirmed = false;
            do
            {
                ConsoleKey response;
                do
                {
                    Console.Write("Do you want to exit the program? [y/n]");
                    response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
                    if (response != ConsoleKey.Enter)
                        Console.WriteLine();

                } while (response != ConsoleKey.Y && response != ConsoleKey.N);
                if (response == ConsoleKey.Y)
                {
                    confirmed = response == ConsoleKey.Y;
                    WriteMessage("exiting the program!");
                    Console.ReadLine();
                    Environment.Exit(1);
                }
                else if (response == ConsoleKey.N)
                {
                    confirmed = true;
                }
            } while (!confirmed);
        }
    }
}

