using System;

namespace ReportChecker
{
    public class ValueChangedEventArgs : EventArgs 
     { 
         public string OldValue { get; set; } 
         public string NewValue { get; set; } 
         public string CalledBy { get; set; } 
     }

}
