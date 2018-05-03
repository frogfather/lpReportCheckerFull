using System;

namespace ReportChecker
{
    public class MatchEventArgs: EventArgs
    {
        public bool Value { get; set; }
        public string CalledBy { get; set; }
    }
}
