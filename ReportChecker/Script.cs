using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportChecker
{
    public class Script
    {

        public Script(string scriptName, Dashboard dash)
        {
            Name = scriptName;
            _dashSuccess = new List<string>();
            _dashFail = new List<string>();
            _emailSuccess = new List<string>();
            _emailFail = new Dictionary<string, string>();
            ScriptError = false;
            EmailError = false;
            SuccessCountMatch = true;
            FailCountMatch = true;
        }

        public string Name { get; set; }
        public int FacId { get; set; }
        public string ScriptStart { get; set; }
        public string ScriptEnd { get; set; }
        public bool EmailError { get; set; }
        public bool SuccessCountMatch { get; set; }
        public bool FailCountMatch { get; set; }
        public bool ScriptError { get; set; }

        public int RecCount
        {  
             get 
             { 
                 return _recCount;    
             }  
             set 
             { 
                if(_recCount != value) 
                 { 
                     ValueChangedEventArgs args = new ValueChangedEventArgs
                     { 
                         OldValue = _recCount.ToString(), 
                         NewValue = value.ToString(), 
                         CalledBy = Name + " Record Count" 
                     }; 
                     if (ValueChanged != null) 
                    { 
                         ValueChanged(this, args); 
                     } 
                     _recCount = value; 
                 } 
             }  
         } 

        public int SuccessCount
         {  
             get 
             { 
                 return _successCount; 
             } 
             set 
             { 
                 if (_successCount != value) 
                 { 
                     ValueChangedEventArgs args = new ValueChangedEventArgs
                     { 
                         OldValue = _successCount.ToString(), 
                         NewValue = value.ToString(), 
                         CalledBy = Name + " Success Count" 
                     }; 
 
 
                     ValueChanged?.Invoke(this, args); 
                     _successCount = value; 
 
 
                 } 
             } 
         } 
 
 
         public int FailCount
         {  
             get 
             { 
                 return _failCount;         
             } 
             set 
             { 
                 if (_failCount != value) 
                 { 
                     ValueChangedEventArgs args = new ValueChangedEventArgs
                     { 
                         OldValue = _failCount.ToString(), 
                         NewValue = value.ToString(), 
                         CalledBy = Name + "Fail count" 
                     }; 
                     ValueChanged?.Invoke(this, args); 
                     _failCount = value; 
 
 
                 } 
             } 
         }
        

        public void AddEmailSuccess(string serviceCode)
 
 
         {             
             if (_emailSuccess.IndexOf(serviceCode) ==-1) 
             { 
                 ValueChangedEventArgs args = new ValueChangedEventArgs
                 { 
                     OldValue = _emailSuccess.Count.ToString(), 
                     NewValue = (_emailSuccess.Count + 1).ToString(), 
                     CalledBy = Name + " Email success codes" 
                 }; 
                 ValueChanged?.Invoke(this, args); 
                 _emailSuccess.Add(serviceCode); 
 
 
                 SuccessCountMatch = CompareSuccess(); 
 
 
                 MatchEventArgs eArgs = new MatchEventArgs()
                 { 
                     Value = SuccessCountMatch, 
                     CalledBy = Name + " SuccessEmail" 
                 }; 
                 MatchChanged?.Invoke(this, eArgs); 
             } 
         } 
 
 
         public string GetEmailSuccess(int index) 
         { 
             if (index< 0 || index >= _emailSuccess.Count) { return ""; } 
             return _emailSuccess[index]; 
         } 
 
 
         public void AddEmailFail(string serviceCode, string errorMsg)
         { 
             if (!_emailFail.ContainsKey(serviceCode)) 
             { 
                 ValueChangedEventArgs args = new ValueChangedEventArgs
                 { 
                     OldValue = _emailFail.Count.ToString(), 
                     NewValue = (_emailFail.Count + 1).ToString(), 
                     CalledBy = Name + " Email fail codes" 
                 }; 
                 ValueChanged?.Invoke(this, args); 
 
 
                 _emailFail[serviceCode] = errorMsg;     
                 FailCountMatch = CompareFail(); 
                 SetEmailErrorFlag(errorMsg);


                MatchEventArgs eArgs = new MatchEventArgs()
                {
                    Value = FailCountMatch,
                    CalledBy = Name + " FailEmail"
                 }; 
                 MatchChanged?.Invoke(this, eArgs); 
 
 
             } 
         } 
 
 
         public string GetEmailFailMessage(string serviceCode)
         { 
             return _emailFail[serviceCode]; 
         }

        public string GetEmailFailCode(int index)
        {
            return _emailFail.Keys.ElementAt(index);
        }
 
         public void AddDashSuccess(string serviceCode)
         { 
             if (_dashSuccess.IndexOf(serviceCode)==-1) 
             { 
                 ValueChangedEventArgs args = new ValueChangedEventArgs
             { 
                 OldValue = _dashSuccess.Count.ToString(), 
                 NewValue = (_dashSuccess.Count + 1).ToString(), 
                 CalledBy = Name + " Dashboard success codes" 
             }; 
                 ValueChanged?.Invoke(this, args); 
 
 
                 _dashSuccess.Add(serviceCode); 
 
 
                 SuccessCountMatch = CompareSuccess(); 
                 MatchEventArgs eArgs = new MatchEventArgs()
                 { 
                     Value = SuccessCountMatch, 
                     CalledBy = Name + " SuccessDash " 
                 }; 
                 MatchChanged?.Invoke(this, eArgs); 
 
 
             } 
         } 
 
 
         public string GetDashSuccess(int index)
         { 
             if (index< 0 || index >= _dashSuccess.Count) { return ""; } 
             return _dashSuccess[index]; 
         } 
 
 
         public void AddDashFail(string serviceCode)
         { 
             if (_dashFail.IndexOf(serviceCode)==-1) 
             { 
                 ValueChangedEventArgs args = new ValueChangedEventArgs
             { 
                 OldValue = _dashFail.Count.ToString(), 
                 NewValue = (_dashFail.Count + 1).ToString(), 
                 CalledBy = Name + " Dashboard fail codes" 
             }; 
                 ValueChanged?.Invoke(this, args); 
 
 
                 _dashFail.Add(serviceCode);     
                 FailCountMatch = CompareFail(); 
 
 
                 MatchEventArgs eArgs = new MatchEventArgs()
                 { 
                     Value = FailCountMatch, 
                     CalledBy = Name+ " FailDash" 
                 }; 
                 MatchChanged?.Invoke(this, eArgs); 
 
 
 
              }                   
         } 
 
 
         public string GetDashFail(int index)
         { 
             if (index< 0 || index >= _dashFail.Count) { return ""; } 
             return _dashFail[index]; 
         }


        public int GetEmailFailCount()
        {
            return _emailFail.Count;
        }

        public int GetEmailSuccessCount()
        {
            return _emailSuccess.Count;
        }

        public int GetDashFailCount()
        {
            return _dashFail.Count;
        }

        public int GetDashSuccessCount()
        {
            return _dashSuccess.Count;
        }

        private bool CompareSuccess()
         { 
             if (_emailSuccess.Count != _dashSuccess.Count) { return false; }     
             foreach(string item in _dashSuccess)
             { 
                 if (_emailSuccess.IndexOf(item)==-1) 
                 { return false; } 
             } 
             return true; 
         }


        private bool CompareFail()
        {
            if (_emailFail.Count != _dashFail.Count) { return false; }
            foreach (string item in _dashFail)
            {
                if (!_emailFail.ContainsKey(item))
                { return false; }
            }
            return true;
        }
 
 
         private void SetEmailErrorFlag(string errorMsg)
         {                         
           
             EmailError |= errorMsg.Contains("ISM");
             if (EmailError) 
             { 
                 MatchEventArgs eArgs = new MatchEventArgs()
                 { 
                     Value = EmailError, 
                     CalledBy = "ISM Error" 
                 }; 
                 MatchChanged?.Invoke(this, eArgs); 
             } 
         }



        public event ValueChangedDelegate ValueChanged;
        public event MatchChangedDelegate MatchChanged;
        int _recCount;
        int _successCount;
        int _failCount;
        List<string> _dashSuccess;
        List<string> _dashFail;
        List<string> _emailSuccess;
        Dictionary<string, string> _emailFail;

    }
}
