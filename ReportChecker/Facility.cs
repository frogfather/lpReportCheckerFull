using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportChecker
{
    public class Facility
    {
        public Facility(string facName)
        {
            Name = facName;            
            RecCount = 0;
            SuccessCount = 0;
            FailCount = 0;
            scripts = new List<Script>();
        }


        private void ReportFailChange(object sender, MatchEventArgs args)
        {
            Console.WriteLine("Script "+ args.CalledBy +"reports status: " + args.Value);
        }

        public void AddScript(string scriptName)
        {
            scripts.Add(new Script(scriptName));
            GetScript(scriptName).MatchChanged += ReportFailChange;
        }

        public Script GetScript(string scriptName)
        {
            if (scripts.Find(x => x.Name == scriptName) == null) { AddScript(scriptName); }
            return scripts.Find(x => x.Name == scriptName);


        }
        //to add check dash success total == email success total, check dash fail total == email fail total, check dash success codes == email success codes, check dash fail codes == email fail codes, check error messages for ISM errors
        public string Name { get; set; }
        public int Server { get; set; }

        public int Coid { get; set; }

        public string Pas { get; set; }

        public int FacId { get; set; }
        public string ScriptingStart { get; set; }
        public string ScriptingEnd { get; set; }
        public int RecCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        List<Script> scripts;
    }
}
