using System;
using System.Collections.Generic;

namespace ReportChecker
{
    public class Facility
    {
        public Facility(string facName, Dashboard dash)
        {
            Name = facName;
            RecCount = 0;
            SuccessCount = 0;
            FailCount = 0;
            Dash = dash;
            scripts = new List<Script>();
        }


        private void ReportFailChange(object sender, MatchEventArgs args)
        {
            bool overallSuccessMatch = true;
            bool overallFailMatch = true;
            bool overallEmailError = false;
            bool overallScriptError = false;
            
            foreach (Script script in scripts)
            {
                //if any facility has a mismatch or ISM error then the flag for this facility is set
                if (script.FailCountMatch == false) { overallFailMatch = false; }
                if (script.SuccessCountMatch == false) { overallSuccessMatch = false; }
                if (script.EmailError == true) { overallEmailError = true; }
                if (script.ScriptError == true) { overallScriptError = true; }
            }
            
            FacilityFailMatch = overallFailMatch;
            FacilitySuccessMatch = overallSuccessMatch;
            FacilityISMError = overallEmailError;
            FacilityScriptError = overallScriptError;
            
            Console.WriteLine("Script " + args.CalledBy + " Facility success: " + overallSuccessMatch);
            Console.WriteLine("Script " + args.CalledBy + " Facility fail: " + overallFailMatch);
            Console.WriteLine("Script " + args.CalledBy + " Email error: " + overallEmailError);
            Console.WriteLine("Script " + args.CalledBy + " Script error: " + overallScriptError);
        }

        public void AddScript(string scriptName)
        {
            if (!ScriptExists(scriptName))
            {
                scripts.Add(new Script(scriptName,Dash));
                GetScript(scriptName).MatchChanged += ReportFailChange;
            }
        }

        private bool ScriptExists(string scriptName)
        {
            return scripts.Find(x => x.Name == scriptName) != null;
        }
        public Script GetScript(string scriptName)
        {
            if (!ScriptExists(scriptName))
            {
                AddScript(scriptName);
            }
            return scripts.Find(x => x.Name == scriptName);
        }
        public Script GetScript(int index)
        {
            if (index >= 0 && index < scripts.Count)
            {
                return scripts[index];            
            }
            else
            {
                return null;
            }
        }

        public int GetScriptCount()
        {
            return scripts.Count;
        }

        //to add check dash success total == email success total, check dash fail total == email fail total, check dash success codes == email success codes, check dash fail codes == email fail codes, check error messages for ISM errors
        public string Name { get; set; }
        public string Server { get; set; }

        public int Coid { get; set; }

        public string Pas { get; set; }

        public int FacId { get; set; }
        public string ScriptingStart { get; set; }
        public string ScriptingEnd { get; set; }
        public int RecCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }

        public bool FacilityFailMatch
        {
            get
            {
                return _facFailMatch;
            }                
            set
            {
                if (_facFailMatch != value)
                {
                    _facFailMatch = value;
                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _facFailMatch,
                        CalledBy = Name + " facility fail match"
                    };
                    FacFlagsChanged?.Invoke(this, eArgs);
                }

            }
        }

        public bool FacilitySuccessMatch
        {
            get
            {
                return _facSuccMatch;
            }
            set
            {
                if (_facSuccMatch != value)
                {
                    _facSuccMatch = value;
                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _facSuccMatch,
                        CalledBy = Name + " facility success match"
                    };
                    FacFlagsChanged?.Invoke(this, eArgs);
                }                
            }
        }

        public bool FacilityISMError
        {
            get
            {
                return _facISMMatch;
            }
            set
            {
                if (_facISMMatch != value)
                {
                    _facISMMatch = value;
                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _facISMMatch,
                        CalledBy = Name + " facility ISM flag"
                    };
                    FacFlagsChanged?.Invoke(this, eArgs);

                }
            }
        }

        public bool FacilityScriptError
        {
            get
            {
                return _facScriptError;
            }
            set
            {
                if (_facScriptError != value)
                {
                    _facScriptError = value;
                    MatchEventArgs eArgs = new MatchEventArgs()
                    {
                        Value = _facScriptError,
                        CalledBy = Name + " facility Script fail flag"
                    };
                    FacFlagsChanged?.Invoke(this, eArgs);

                }
            }
        }

        public Dashboard Dash { get => _dash; set => _dash = value; }

        List<Script> scripts;
        bool _facFailMatch;
        bool _facSuccMatch;
        bool _facISMMatch;
        bool _facScriptError;
        Dashboard _dash;
        public event MatchChangedDelegate FacFlagsChanged;
    }
}
