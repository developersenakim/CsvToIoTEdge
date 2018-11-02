

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
