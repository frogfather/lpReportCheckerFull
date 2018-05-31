using System.IO;
using System.Collections.Generic;

namespace ReportChecker
{
    static class FileUtilities
    {

        public static List<string> ReadFile(string fileName)
        {
            List<string> resultList = new List<string>();
            StreamReader sr = FileUtilities.GetReader(filename);
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

        public static StreamReader GetReader(string filename)
        {
            if (!File.Exists(filename)) { return null; }
            return new StreamReader(filename);
        }


        
        
        
                    
                    


    }
}
