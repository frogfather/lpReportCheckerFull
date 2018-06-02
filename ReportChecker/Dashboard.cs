using System;
using System.Linq;
using System.Collections.Generic;

namespace ReportChecker
{
    public class Dashboard
    {
        private const string IgnoredFile = "C:\\lpReports\\ignored.txt";
        private const string CloudFile = "C:\\lpReports\\cloud.txt";

        public Dashboard()
        {
            facilities = new List<Facility>();
            emailToDash = new Dictionary<string, string>();
            SetUpEmailToDash();
            DashFailMatch = true;
            DashSuccessMatch = true;
            DashISMError = false;
            _ignored = LoadIgnoredList(IgnoredFile);
            _previousCloudNumbers = LoadPreviousCloudNumbers(CloudFile);

            //below is for checking
            //DateTime date1 = new DateTime(2018, 06, 02);
            //DateTime date2 = new DateTime(2018, 06, 03);
            //DateProcessed = date2;
            //FindLastCloudNumbers();
            //SavePreviousCloudNumbers();
;        }

        public DateTime DateProcessed { get; set; }

        private List<string> LoadIgnoredList(string fileName)
        {
            return FileUtilities.ReadFile(fileName);
        }

        public void SaveIgnoredList()
        {                        
            FileUtilities.WriteFile(IgnoredFile, _ignored);
        }

        public void AddToIgnoredList(string errMessage)
        {
            if (!InIgnoredList(errMessage))
            {
                _ignored.Add(errMessage);
            }
        }

        public bool InIgnoredList(string errMessage)
        {
            return (_ignored.IndexOf(errMessage) > -1);            
        }

        public void RemoveFromIgnoredList(string errMessage)
        {
            if (!InIgnoredList(errMessage)) { return; }
            _ignored.RemoveAt(_ignored.IndexOf(errMessage));
        }

        private Dictionary<string,string> LoadPreviousCloudNumbers(string fileName)
        {
            Dictionary<string, string> dateList = new Dictionary<string, string>();
            List<string> fileLines = new List<string>();
            char[] separator = { ','};
            fileLines = FileUtilities.ReadFile(fileName);
            foreach(string line in fileLines)
            {
               string[] lineParts =  line.Trim().Split(separator);
               if (lineParts.Length == 2)
                {
                    if (lineParts[0].Trim() != "" && lineParts[1].Trim() != "")
                    {
                        dateList.Add(lineParts[0].Trim(), lineParts[1].Trim());
                    }
                }
            }
            return dateList;
        }

        public void SavePreviousCloudNumbers()
        {
            FileUtilities.WriteFile(CloudFile, _previousCloudNumbers);
        }

        public void AddToPreviousCloudNumbers(string date, string count)
        {
            if (!_previousCloudNumbers.ContainsKey(date))
            {
                _previousCloudNumbers.Add(date, count);
            }
        }

        public bool RemoveFromPreviousCloudNumbers(string date)
        {
            return _previousCloudNumbers.Remove(date);
        }

        public int FindLastCloudNumbers()
        {
            string dayBefore = String.Format("{0:dd/MM/yyyy}" ,DateProcessed.AddDays(-1));
            
            string cloudCount;
            //find the date before the date we're checking           
            if (_previousCloudNumbers.ContainsKey(dayBefore))
            {               
                cloudCount = _previousCloudNumbers[dayBefore];
                return Convert.ToInt32(cloudCount);
            }
            return -1;
        }



        private void SetUpEmailToDash()
        {
            emailToDash.Clear();                                                           
            emailToDash.Add("Memorial Medical Center - Las Cruces", "Memorial Medical Center Of Las Cruces");                                    
            emailToDash.Add("SageWest Health Care - Lander Campus", "SageWest Health Care - Lander");                        
            emailToDash.Add("Starr Regional Medical Center - Athens", "Starr Regional Medical Center - Athens campus");                                               
            emailToDash.Add("Fleming County Hospital District", "Fleming County Hospital");                                              
            emailToDash.Add("Danville Regional Medical", "Danville Regional Medical Center");                                                                      
            emailToDash.Add("Vaughan Regional Medical Center", "Vaughan Regional Medical Center Parkway Campus");
        }

        public void ProcessEmails(List<string> results, string facilityName, bool success)
         {
            string[] sepItems = { };
            int scPos = -1;
            int fmPos = -1;
            string serviceCode = "";
            string errMsg = "";
            string scriptName = "";
            //first we need to add the facility 
            facilityName = TranslateIfNecessary(facilityName);
            AddFacility(facilityName);             
             //now we need to find the script names 
             foreach(string result in results) 
             {
                string[] separators = { "," };
                sepItems = result.Split(separators, StringSplitOptions.None).Select(p => p.Trim()).ToArray();

                if (result.Contains("Script Name:"))
                 {
                    //extract the script name
                    scriptName = result.Substring(result.LastIndexOf(":") + 1).Trim(); 
                    GetFacility(facilityName).AddScript(scriptName);
                 }
                 else if (result.Contains("Service Code")) 
                 { 
                     scPos = Array.IndexOf(sepItems, "Service Code"); 
                     fmPos = Array.IndexOf(sepItems, "Reason for error");  //trim                    
                 } 
                 else if (scriptName != "" && scPos > -1)
                 {
                    serviceCode = sepItems[scPos];
                    if (success)
                    {
                        GetFacility(facilityName).GetScript(scriptName).AddEmailSuccess(serviceCode);
                    }
                    else
                    {
                        if (fmPos > -1) { errMsg = sepItems[fmPos]; }
                        GetFacility(facilityName).GetScript(scriptName).AddEmailFail(serviceCode, errMsg);
                    }
                 }
                 else
                 {
                    //something wrong?
                 }
                 
             } 
 
 
         } 
 
        public void ProcessDashboard(List<string> results)
        {
            //start with the last sheet. 
            //find line 1
            string[] headers = { };
            string[] sepResults;
            string[] parameters = { "\t" };
            string[] commaSplit = { "," };
            char[] trimParams = { '[', ']' };
            string currentFacilityName = "";
            string currentServer = "";
            bool detail = false;
            bool summary = false;
            bool itemsToCheck = false;

            foreach (string item in results)
            {                    
                sepResults = item.Split(parameters, StringSplitOptions.None);
                //is this enough of a check? Can check page no too.
                if (sepResults[1] == "Server" || sepResults[1] == "Name" || sepResults[1] == "PAS / HCIS")
                {
                    headers = item.Split(parameters, StringSplitOptions.None);
                    detail = sepResults[0] == "Detail";
                    summary = sepResults[0] == "Detail"; //put correct value here
                }
                else
                {
                    //process the records depending on whether they're detail, summary or server page
                    if (detail)
                    {                                                                                                                        
                        string facilityName = GetDataFromColumn(headers, sepResults, "Name").Trim();
                        string scriptName = GetDataFromColumn(headers, sepResults, "Script Name").Trim();
                        
                        if (GetDataFromColumn(headers, sepResults, "Server")!="")
                        {
                            currentServer = GetDataFromColumn(headers, sepResults, "Server");
                        }
                        if ( facilityName != "")
                        {
                            itemsToCheck = GetDataFromColumn(headers, sepResults, "Record Count") != "0";

                            //add the facility if it's not there
                            if (itemsToCheck)
                            {
                                currentFacilityName = facilityName; //change to currentFacilityName
                                AddFacility(facilityName);
                                GetFacility(facilityName).RecCount = Convert.ToInt16(GetDataFromColumn(headers, sepResults, "Record Count"));
                                GetFacility(facilityName).SuccessCount = Convert.ToInt16(GetDataFromColumn(headers, sepResults, "Success Count"));
                                GetFacility(facilityName).FailCount = Convert.ToInt16(GetDataFromColumn(headers, sepResults, "Failure Count"));
                                GetFacility(facilityName).Server = currentServer;
                            }
                        }
                        else if (itemsToCheck && GetDataFromColumn(headers, sepResults, "Script Name") != "Scripting")
                        {
                            //add scripts
                            GetFacility(currentFacilityName).GetScript(scriptName);
                            try
                            {
                                GetFacility(currentFacilityName).GetScript(scriptName).RecCount += Convert.ToInt16(GetDataFromColumn(headers, sepResults, "Record Count"));
                                GetFacility(currentFacilityName).GetScript(scriptName).SuccessCount += Convert.ToInt16(GetDataFromColumn(headers, sepResults, "Success Count"));
                                GetFacility(currentFacilityName).GetScript(scriptName).FailCount += Convert.ToInt16(GetDataFromColumn(headers, sepResults, "Failure Count"));
                            }
                            catch (FormatException)
                            {
                                //these columns should have a value. if they don't - poss script fail
                                if (scriptName != "")
                                {
                                    GetFacility(currentFacilityName).GetScript(scriptName).ScriptError = true;
                                    Console.WriteLine("Exception in " + currentFacilityName + " script " + scriptName);
                                }
                                else
                                {
                                    GetFacility(currentFacilityName).FacilityScriptError = true;
                                    Console.WriteLine("Exception in " + currentFacilityName + " unknown script ");
                                }
                                
                            }

                            string successCodes = GetDataFromColumn(headers, sepResults, "Success Service Codes");
                            string failCodes = GetDataFromColumn(headers, sepResults, "Failure Service Codes");

                            if (successCodes != "[]")
                            {
                                successCodes = successCodes.Trim(trimParams).Replace("\"", "");                                              
                                string[] sepSuccessCodes = successCodes.Split(commaSplit, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string entry in sepSuccessCodes)
                                {                                  
                                    GetFacility(currentFacilityName).GetScript(scriptName).AddDashSuccess(entry);
                                }
                                                                    
                            }

                            if (failCodes != "[]")
                            {
                                failCodes = failCodes.Trim(trimParams).Replace("\"", "");
                                string[] sepFailCodes = failCodes.Split(commaSplit, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string entry in sepFailCodes)
                                {                                 
                                    GetFacility(currentFacilityName).GetScript(scriptName).AddDashFail(entry);
                                }                                
                            }
                        }
                        
                        
                    }
                    else if (summary)
                    {

                    }
                    else
                    {
                        //server summary
                    }
                }
                
                

            }
            
        }
 
        private string TranslateIfNecessary(string key)
        {
            key = key.Trim();
            if (emailToDash.ContainsKey(key))
            {
                return emailToDash[key];
            }
            else
            {
                return key;
            }
        }

        public string GetDataFromColumn(string[] header, string[] data, string heading)
        {
            int index = Array.FindIndex(header, x => x == heading);
            if (index == -1) { return "";}
            return data[index];
                
            
        }
            
        public bool FacilityExists(string facilityName)
         { 
             return facilities.Find(x => x.Name.ToUpper() == facilityName.ToUpper()) != null; 
         } 
 
        public void AddFacility(string facilityName)
         { 
            if (!FacilityExists(facilityName))
            {
                facilities.Add(new Facility(facilityName));
                Console.WriteLine("Adding facility " + facilityName);
                Console.WriteLine("Facility count is now " + GetFacilityCount());
                GetFacility(facilityName).FacFlagsChanged += ReportFacFlagsChanged;
            }
             
         } 
 
        public Facility GetFacility(int index)
        {
            if (index >= 0 && index < facilities.Count)
            {
                return facilities[index];
            }
            else
            {
                return null;
            }
        }
 
        public Facility GetFacility(string facilityName)
         { 
            if (!FacilityExists(facilityName))
            {
                AddFacility(facilityName);
            }
             return facilities.Find(x => x.Name.ToUpper() == facilityName.ToUpper());     
         } 
 
        public int GetFacilityCount()
        {
            return facilities.Count;
        }
 
        private void ReportFacFlagsChanged(object sender, MatchEventArgs args)
        {
            //need to check all facility flags
            bool dashFailMatch = true;
            bool dashSuccMatch = true;
            bool dashISMError = false;
            bool dashScriptError = false;

            foreach (Facility facility in facilities)
            {
                if (facility.FacilitySuccessMatch == false) { dashSuccMatch = false; }
                if (facility.FacilityFailMatch == false) { dashFailMatch = false; }
                if (facility.FacilityISMError == true) { dashISMError = true; }
                if (facility.FacilityScriptError == true) { dashScriptError = true; }


            }
            DashFailMatch = dashFailMatch;
            DashSuccessMatch = dashSuccMatch;
            DashISMError = dashISMError;
            DashScriptError = dashScriptError;

            Console.WriteLine("Facility " + args.CalledBy + " dash success match: " + dashSuccMatch);
            Console.WriteLine("Facility " + args.CalledBy + " dash fail match: " + dashFailMatch);
            Console.WriteLine("Facility " + args.CalledBy + " dash ISM Error: " + dashISMError);
            Console.WriteLine("Facility " + args.CalledBy + " dash Script Error: " + dashScriptError);
        }

        public bool DashFailMatch
        {
            get
            {
                return _dashFailMatch;
            }
            set
            {
                if (_dashFailMatch != value)
                {
                    _dashFailMatch = value;

                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _dashFailMatch,
                        CalledBy = " Dashboard fail match"
                    };
                    DashFlagsChanged?.Invoke(this, eArgs);
                }

            }
        }

        public bool DashSuccessMatch
        {
            get
            {
                return _dashSuccMatch;
            }
            set
            {
                if (_dashSuccMatch != value)
                {
                    _dashSuccMatch = value;
                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _dashSuccMatch,
                        CalledBy = " Dashboard success match"
                    };
                    DashFlagsChanged?.Invoke(this, eArgs);

                }

            }
        }

        public bool DashISMError
        {
            get
            {
                return _dashISMError;
            }
            set
            {
                if (_dashISMError != value)
                {
                    _dashISMError = value;
                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _dashISMError,
                        CalledBy = " Dashboard ISM error"
                    };
                    DashFlagsChanged?.Invoke(this, eArgs);

                }

            }
        }

        public bool DashScriptError
        {
            get
            {
                return _dashScriptError;
            }
            set
            {
                if (_dashScriptError != value)
                {
                    _dashScriptError = value;
                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _dashScriptError,
                        CalledBy = " Dashboard Script error"
                    };
                    DashFlagsChanged?.Invoke(this, eArgs);

                }

            }
        }



        List<Facility> facilities;
        Dictionary<string, string> emailToDash;

        bool _dashFailMatch;
        bool _dashSuccMatch;
        bool _dashISMError;
        bool _dashScriptError;
        
        List<string> _ignored = new List<string>();
        Dictionary<string, string> _previousCloudNumbers = new Dictionary<string, string>();
        public event MatchChangedDelegate DashFlagsChanged;
    }

}
