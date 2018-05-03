using System;
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
            if (!FacilityExists(facilityName)) 
             { 
                 AddFacility(facilityName); 
             } 
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
            
            Console.WriteLine("Test");
        }
 
         public bool FacilityExists(string facilityName)
         { 
             return facilities.Find(x => x.Name == facilityName) != null; 
         } 
 
 
         public void AddFacility(string facilityName)
         { 
             facilities.Add(new Facility(facilityName)); 
         } 
 
 
         public Facility GetFacility(string facilityName)
         { 
             return facilities.Find(x => x.Name == facilityName);     
         } 
 
 

 
         List<Facility> facilities;     


    }

}
