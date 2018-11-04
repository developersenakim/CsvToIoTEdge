

using System;
using System.Threading;
using System.Diagnostics;

namespace CsvToIoTEdge
{
    class Program
    {
        static void Main(string[] args)
        {
            Application application = new Application();
            application.Init();
            application.Process();
            //for (int i = 0; i < 10; i++)
            //{
            //    LogBuilder.DrawProgressBar(i, 10);

            //    if (i == 9)
            //    {
            //        Console.Write("Done ");
            //    }
            //}





            Console.Read();
            

        }

    }
}




//using System;

//namespace CsvToIoTEdge
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {

//            Console.CursorVisible = false;

//            ThreadPool threadpool = new ThreadPool();
//            //############################ Assign global variables##################
//            threadpool.Initialization();

//            Console.ReadLine();
//        }

//    }
//}
