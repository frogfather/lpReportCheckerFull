using System;
using System.IO;
using System.Collections.Generic;

namespace ReportChecker
{
    static class FileUtilities
    {

        public static List<string> ReadFile(string fileName)
        {
            List<string> resultList = new List<string>();
            StreamReader sr = FileUtilities.GetReader(fileName);
            if (sr != null)
            {
                using (sr)
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        resultList.Add(line);
                    }
                }
            }
                    return resultList;
        }

        public static StreamReader GetReader(string fileName)
        {
            if (!File.Exists(fileName)) { return null; }
            return new StreamReader(fileName);
        }

        public static StreamWriter GetWriter(string fileName)
        {
            return new StreamWriter(fileName);
        }

        public static void WriteFile(string fileName, List<string> dataToWrite)
        {            
            StreamWriter sw = GetWriter(fileName);
            using (sw)
            {
                if (dataToWrite.Count == 0) { return; }
                foreach (string line in dataToWrite)
                {
                    sw.WriteLine(line);
                }

            }
        }

        public static void WriteFile(string fileName, Dictionary<string,string> dataToWrite)
        {
            StreamWriter sw = GetWriter(fileName);
            using (sw)
            {
                if (dataToWrite.Count == 0) { return; }
                foreach (KeyValuePair<string,string> kvp in dataToWrite)
                {
                    sw.WriteLine(kvp.Key + ',' + kvp.Value);
                }

            }
        }

        public static string GetDirectoryFromFilePath(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf("\\") + 1);
        }


        public static DateTime GetFileDate(string fileName) //should get from name if excel in case generated manually?
        {
            if (File.Exists(fileName))
            {
                return File.GetLastWriteTime(fileName);
            }
            else
            {
                return Convert.ToDateTime("11/11/2222");
            }
        }




    }
}
