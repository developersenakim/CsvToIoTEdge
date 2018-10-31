using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Collections;

namespace CsvToIoTEdge
{
    public class Application
    {
        public static string s_sqlconnectionstring;
        public static string s_folderPath;
        public static Queue<string> q_csvfilename;

        public Application()
        {
            
        }

}

    public class Pretask
    {
        ///<summary>
        ///* Function: Metod which sets a queue of string that is passed 
        ///* @author: Sena.kim
        ///* @parameter: fileType array of string 
        ///* @return: none
        ///</summary>
        public void SetQueueFileName(FileInfo[] collection, ref Queue<string> newcollector)
        {
            LogBuilder.WriteMessage("Setting" + collection.Length + "files");
            Queue<string> temp_queue = new Queue<string>();
            if (collection.Length == 0)
            {
                LogBuilder.WriteErrorMessage("There is no exisitng csv file");
            }
            foreach (FileInfo fi in collection)
            {
               // LogBuilder.WriteMessage(" " + fi.Name.ToString() + Environment.NewLine);
                temp_queue.Enqueue(fi.Name.ToString());
            }
            newcollector = temp_queue;
            LogBuilder.WriteMessage("QueueisSet"+ newcollector.Count);
        }
        ///<summary>
        ///* Function: Check if the folder has the file type. returns all the file names 
        ///* @author: Sena.kim
        ///* @parameter: fileType string ("*csv")
        ///* @return: Array of fileinfos
        ///</summary>
        public FileInfo[] Readfromfolder(string fileType, string folderPath)
        {
            FileInfo[] fi;
            fi = new FileInfo[1];

            if (System.IO.Directory.Exists(folderPath))
            {
                //Check if the CSV files exist.
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderPath);
                fi = di.GetFiles(fileType);

                if (fi.Length == 0)
                {
                    string message = "There is no existing " + fileType + " files. \nIn " + folderPath;
                    LogBuilder.WriteErrorMessage(message);
                }
                else
                {
                    LogBuilder.WriteMessage("In " + folderPath + "folder." + "\nDetecting " + fi.Length + " " + fileType + " files.");
                }
            }
            else
            {
                LogBuilder.WriteErrorMessage("The folder does not exist. \nChange folder path from config.txt");
            }
            return fi;
        }
        ///<summary>
        ///* Function: function to read from content from a file using splitstring to detect the line to get a string
        ///* @author: Sena.kim
        ///* @parameter: filepath (string),  string splitstring, ref string var
        ///* @return: none. assigns the string 
        ///</summary>
        public void ReadContentfromFile(string filepath, string splitstring, ref string var)
        {
            if (System.IO.File.Exists(filepath))
            {
                string[] lines = System.IO.File.ReadAllLines(filepath);
                // System.Console.WriteLine("Contents of WriteLines2.txt = ");
                if (System.IO.File.Exists(filepath))
                {
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(splitstring))
                        {
                            string tempstring = line.Split(splitstring)[1];
                            tempstring = tempstring.TrimStart();
                            tempstring = tempstring.TrimEnd();
                            var = tempstring;
                        }
                    }
                }
                else
                {
                    LogBuilder.WriteErrorMessage("the file :'" + filepath + "'\tdoes not exist");
                }
            }
        }
        ///<summary>
        ///* Function: Sets global variable as pretask 
        ///* @author: Sena.kim
        ///* @parameter: none
        ///* @return: none.
        ///</summary>
        public void Initialization()
        {
            ReadContentfromFile(@"../../../config.txt", "SQLconnectionString:", ref Application.s_sqlconnectionstring); ;
            ReadContentfromFile(@"../../../config.txt", "CSVfolderPath:", ref Application.s_folderPath);
            SetQueueFileName(Readfromfolder("*.csv", Application.s_folderPath), ref Application.q_csvfilename);
        }
    }
   
    class Tasks
    {

    }
}
