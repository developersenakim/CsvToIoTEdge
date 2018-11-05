using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace CsvToIoTEdge
{
    class LogBuilder
    {
        static public void DrawProgressBar(int progress_current, int progress_max)
        {
            using (var progress = new ProgressBar())
            {
                progress.Report((double)progress_current / progress_max);
                Thread.Sleep(1);
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

