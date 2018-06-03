using System;
using NUnit.Framework;

namespace ReportChecker
{
    [TestFixture]
    public class ScriptClassTest
    {
         [Test()] 
         public void ItSetsISMErrorFlagTrueWhenErrorMessageIsISM()
         { 
             Script testScript = new Script("TestScript",null); 
             testScript.AddEmailFail("1234", "ISM"); 
             Assert.IsTrue(testScript.EmailError); 
         } 
 
 
         [Test()] 
         public void ItSetsISMErrorFlagTrueWhenErrorMessageContainsISM()
         { 
             Script testScript = new Script("TestScript",null); 
             testScript.AddEmailFail("1234", "Thing containing ISM: error"); 
             Assert.IsTrue(testScript.EmailError); 
         } 
 
 
         [Test()] 
         public void ItSetsISMErrorFlagFalseWhenErrorMessageDoesNotContainsISM()
         { 
             Script testScript = new Script("TestScript",null); 
             testScript.AddEmailFail("1234", "Thing containing another error"); 
             Assert.IsFalse(testScript.EmailError); 
         } 
 
 
         [Test()] 
         public void ItWillNotAddADuplicateChargeCode()
         { 
             Script testScript = new Script("TestScript",null); 
             testScript.AddEmailFail("1234", "Error Message1"); 
             testScript.AddEmailFail("1234", "Error Message2"); 
             Assert.AreEqual("Error Message1",testScript.GetEmailFailMessage("1234")); 
         } 
 
 
         [Test()] 
         public void ItWillAddDifferentChargeCodes()
         { 
             Script testScript = new Script("TestScript",null); 
             testScript.AddEmailFail("1234", "Error Message4"); 
             testScript.AddEmailFail("1235", "Error Message2"); 
             Assert.AreEqual("Error Message4", testScript.GetEmailFailMessage("1234")); 
             Assert.AreEqual("Error Message2", testScript.GetEmailFailMessage("1235")); 
         } 

    }
}
