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
        protected string s_sqlconnectionstring;
        protected string s_folderPath;
        protected Queue<string> q_csvfilename;
        protected string SqlconnectionState;

        public Application()
        {
        }

        public void Init()
        {
            ReadContentfromFile(@"../../../config.txt", "SQLconnectionString:", ref s_sqlconnectionstring); ;
            ReadContentfromFile(@"../../../config.txt", "CSVfolderPath:", ref s_folderPath);
            SetQueueFileName(Readfromfolder("*.csv", s_folderPath), ref q_csvfilename);
        }
        public void Process()
        {  
            
            SQLClass sqlclass = new SQLClass(GetConnectionString());  
            sqlclass.CheckSqlConnection();


            //do while task complete check. 
            Tasks tasks = new Tasks();
            tasks.s_filepath = this.SetfilepathAndDeQue(ref tasks);
            tasks.Process(sqlclass);   
        }

        public string GetConnectionString()
        {
            if (s_sqlconnectionstring.Length == 0)
            {
                LogBuilder.WriteMessage("connectionString Empty");
            }

            return s_sqlconnectionstring;
        }

        ///<summary>
        ///* Function: Sets private variable filename inside the task. returns the file path of the file name.
        ///* @author: Sena.kim
        ///* @parameter: none
        ///* @return: filepath.
        ///</summary>
        public string SetfilepathAndDeQue(ref Tasks tasks)
        {
            string temp_filepath;
            string temp_filename = q_csvfilename.Dequeue();
            if (s_folderPath.EndsWith(@"\"))
            {
                temp_filepath = s_folderPath + temp_filename;

            }
            else
            {
                temp_filepath = s_folderPath + "\\" + temp_filename;
            }
            tasks.s_filename = temp_filename;
            return temp_filepath;
        }
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
            LogBuilder.WriteMessage("QueueisSet" + newcollector.Count);
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

    }

    public class Tasks
    {
        public string s_filepath;
        public string s_filename;
        public bool b_fileexist = false;
        DataInfo d_datainfo; 

        public struct DataInfo
        {
            public MainData Maindata;
            public ValueData X1;
            public ValueData X2;
            public ValueData X3;
        }

        public struct MainData
        {
            public string Barcord;
            public string Line;
            public DateData Date;
            public TimeData Time;
            public string Type; 
                }
        public struct DateData
        {
            public short Year;
            public short Month;
            public short Date; 
        }
        public struct TimeData
        {
            public short Hour;
            public short Minute;
            public short Second;
        }

        public struct ValueData
        {
            public double Max;
            public double Min;
            public double Avg;
            public double Std;
        }

        public Tasks()

        {            
        }
        public void Initialization()
        {

        }

        public void Process(SQLClass sqlclass)
        {
            if (DoesFileExist() == true)
            {
                char[] delimiterChars = { '-', '_','.' };
                AssignDatainfoUsingfilename(s_filename, delimiterChars);
                //SQL work start here
                string temp_filenameString = s_filename.Replace(".csv","");
                temp_filenameString = temp_filenameString.Replace("-", "_");
                //SQL check if table exist and create table if it does not exist
                LogBuilder.WriteMessage("About the table <" + temp_filenameString + ">");
                if (!sqlclass.CheckTableNameInSQL(temp_filenameString))
                {
                    LogBuilder.WriteMessage("The table does not exist");
                    sqlclass.CreateTableInSQL(temp_filenameString);
                }
                //Check if table exist Insert Data
                if (sqlclass.CheckTableNameInSQL(temp_filenameString))
                {                    
                    LogBuilder.WriteMessage("The table already exist");
                    string[] csvRows = System.IO.File.ReadAllLines(s_filepath);
                    double max_number = csvRows.Length - 1;
                    Console.Write("Inserting Data into the table ");
                    for (int i = 0; i < csvRows.Length; i++)
                    {
                        double percentage = (double)(((i ) * 100) / max_number);
                        //LogBuilder.WriteMessage(" " + (int)percentage);
                        sqlclass.InsertRawDataInSQL(csvRows[i], temp_filenameString);
                      //  LogBuilder.DrawProgressBar(percentage, 100); //thread this
                    }
                }
                //check if all data has been filled
                // move file to diffrent folder
                // get AVG STANDARD...
                // Assign to variables of datainfo
                // Create new Table if the table exists fill it up with data.
                LogBuilder.WriteMessage(" Done ");
            }
        }

        ///<summary>
        ///* Function: Sets & Gets private variable of bool check if filepath inside task class exist
        ///* @author: Sena.kim
        ///* @parameter: none
        ///* @return: bool b_fileexist
        ///</summary>
        public bool DoesFileExist()
        {
            b_fileexist = false;
            if (File.Exists(s_filepath))
            {
                b_fileexist = true;
            }
            return b_fileexist;
        }

        ///<summary>
        ///* Function: Splits filename using splitstring
        ///* @author: Sena.kim
        ///* @parameter: filename split phrase to split. phr
        ///* @return: none
        ///</summary>
        private void AssignDatainfoUsingfilename(string filename, char[] delimiterchars)
        {
            string[] words = filename.Split(delimiterchars);

            d_datainfo.Maindata.Barcord = words[0];
            d_datainfo.Maindata.Line = words[1];
            d_datainfo.Maindata.Date.Year = ParseStringToTypeShort(words[2]);
            d_datainfo.Maindata.Date.Month = ParseStringToTypeShort(words[3]);
            d_datainfo.Maindata.Date.Date = ParseStringToTypeShort(words[4]);
            d_datainfo.Maindata.Time.Hour = ParseStringToTypeShort(words[5]);
            d_datainfo.Maindata.Time.Minute = ParseStringToTypeShort(words[6]);
            d_datainfo.Maindata.Time.Second = ParseStringToTypeShort(words[7]);
            d_datainfo.Maindata.Type = words[8];
        }

        public short ParseStringToTypeShort(string parseString)
        {
            short temp = 0;
            try
            {
                temp = short.Parse(parseString);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            return temp;
        }


    }
}
