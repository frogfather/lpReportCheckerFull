using System;
using System.Collections.Generic;
using System.IO;


namespace ReportChecker
{
    public class FileReader
    {
        public FileReader()
        {
            _contents = new List<string>();
        }


        private void AddResults(string input)
        {
            _contents.Add(input);
        }


        public List<string> GetResults()
        {
            return _contents;
        }
        public string FacilityName
        {
            get
            {
                return _facilityName;
            }


            set
            {
                _facilityName = value;
            }
        }


        public bool Success
        {
            get
            {
                return _success;
            }
            set
            {
                _success = value;
            }
        }




        public void ReadFile(string filename)
        {
            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;
                    string prevLine = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!LineIsBlank(line))
                        {
                            if (prevLine.Contains("<div id=facilityBatchHeader>"))
                            {
                                FacilityName = HTMLToCSV(line);
                            }
                            if (prevLine.Contains("<div id=resultsHeader>"))
                            {
                                line = HTMLToCSV(line);
                                Success = (line.Contains("Success"));
                            }
                            if (line.Contains("<div id=facilityBatchContent>"))
                            {
                                string[] sepTables;
                                string[] sepRows;

                                sepTables = SplitStringByTable(line.Trim());
                                foreach (string table in sepTables)
                                {
                                    sepRows = SplitStringByTableRow(table);
                                    foreach (string row in sepRows)
                                    {
                                        string csvResults = HTMLToCSV(row);
                                        if (csvResults.Length > 0)
                                        {
                                            AddResults(csvResults);
                                        }
                                        
                                    }
                                }
                            }
                            prevLine = line;
                        }
                    }
                }
            }
        }

        private bool LineIsBlank(string line)
        {
            line.Replace(" ", "");
            return line.Length == 0;
        }

        private string[] SplitStringByTable(string input)
        {
            string[] separators = { "</table>" };
            return input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        private string[] SplitStringByTableRow(string input)
        {
            string[] separators = { "</tr>" };
            return input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }


        private string HTMLToCSV(string input)
        {
            string outputString = "";
            int previousOpeningTagPos = 0;
            int nextClosingTagPos = 0;
            string endTag;
            if (input.IndexOf(">", 0, StringComparison.Ordinal) == -1) { return input; }
            input = input.Trim();
            if (input.IndexOf("</td>") > -1)
            {
                endTag = "</td>";
            }
            else if (input.IndexOf("</th>") > -1)
            {
                endTag = "</th>";
            }
            else
            {
                endTag = "</";
            }
            while (nextClosingTagPos > -1 && previousOpeningTagPos > -1)
            {
                if (previousOpeningTagPos > -1) {nextClosingTagPos = input.IndexOf(endTag, nextClosingTagPos, StringComparison.Ordinal) + endTag.Length - 1;}
                if (nextClosingTagPos-endTag.Length > 0) { previousOpeningTagPos = input.LastIndexOf(">", nextClosingTagPos - endTag.Length, StringComparison.Ordinal); }
                if (nextClosingTagPos-endTag.Length > 0 && previousOpeningTagPos > 0)                    
                {
                    if (outputString.Length > 0) { outputString += ", "; }
                    outputString += input.Substring(previousOpeningTagPos + 1, nextClosingTagPos - (previousOpeningTagPos + endTag.Length));
                        
                }
                else
                {
                    nextClosingTagPos = -1;
                }
                
            }
            return outputString;
        }


        String _facilityName;
        bool _success;
        List<string> _contents;
    }
}
