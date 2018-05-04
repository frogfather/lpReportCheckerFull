﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace ReportChecker
{
    public class Dashboard
    {
        
        public Dashboard()
        {
            facilities = new List<Facility>();
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
            AddFacility(facilityName);             
             //now we need to find the script names 
             foreach(string result in results) 
             {
                string[] separators = { "," };
                sepItems = result.Split(separators, StringSplitOptions.None).Select(p => p.Trim()).ToArray();

                if (result.Contains("Script Name:"))
                 {
                    //extract the script name
                    scriptName = result.Substring(result.LastIndexOf(":") + 1); //use the split item instead
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
            string currentFacilityName = "";
            int currentServer = 0;
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
                    detail = sepResults[1] == "Server";
                    summary = sepResults[1] == "PAS / HCIS";
                }
                else
                {
                    //process the records depending on whether they're detail, summary or server page
                    if (detail)
                    {                                                                                                                        
                        string facilityName = GetDataFromColumn(headers, sepResults, "Name");
                        string scriptName = GetDataFromColumn(headers, sepResults, "Script Name");
                        
                        if (GetDataFromColumn(headers, sepResults, "Server")!="")
                        {
                            currentServer = Convert.ToInt16(GetDataFromColumn(headers, sepResults, "Server"));
                        }
                        if ( facilityName != "")
                        {
                            itemsToCheck = GetDataFromColumn(headers, sepResults, "Record Count") != "0";

                            //add the facility if it's not there
                            if (itemsToCheck)
                            {
                                currentFacilityName = facilityName;
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
                                string successCodes = GetDataFromColumn(headers, sepResults, "Success Service Codes");
                                string failCodes = GetDataFromColumn(headers, sepResults, "Failure Service Codes");

                                if (successCodes != "")
                                {
                                    //add success codes. Should be a way to merge the arrays?
                                }
                                if (failCodes != "")
                                {
                                    //add fail codes.
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
 
        public string GetDataFromColumn(string[] header, string[] data, string heading)
        {
            int index = Array.FindIndex(header, x => x == heading);
            if (index == -1) { return "";}
            return data[index];
                
            
        }
            
         public bool FacilityExists(string facilityName)
         { 
             return facilities.Find(x => x.Name == facilityName) != null; 
         } 
 
 
         public void AddFacility(string facilityName)
         { 
            if (!FacilityExists(facilityName))
            {
                facilities.Add(new Facility(facilityName));
            }
             
         } 
 
 
         public Facility GetFacility(string facilityName)
         { 
             return facilities.Find(x => x.Name == facilityName);     
         } 
 
 

 
         List<Facility> facilities;     


    }

}
