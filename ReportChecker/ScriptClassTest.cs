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
             Script testScript = new Script("TestScript"); 
             testScript.AddEmailFail("1234", "ISM"); 
             Assert.IsTrue(testScript.ISMError); 
         } 
 
 
         [Test()] 
         public void ItSetsISMErrorFlagTrueWhenErrorMessageContainsISM()
         { 
             Script testScript = new Script("TestScript"); 
             testScript.AddEmailFail("1234", "Thing containing ISM: error"); 
             Assert.IsTrue(testScript.ISMError); 
         } 
 
 
         [Test()] 
         public void ItSetsISMErrorFlagFalseWhenErrorMessageDoesNotContainsISM()
         { 
             Script testScript = new Script("TestScript"); 
             testScript.AddEmailFail("1234", "Thing containing another error"); 
             Assert.IsFalse(testScript.ISMError); 
         } 
 
 
         [Test()] 
         public void ItWillNotAddADuplicateChargeCode()
         { 
             Script testScript = new Script("TestScript"); 
             testScript.AddEmailFail("1234", "Error Message1"); 
             testScript.AddEmailFail("1234", "Error Message2"); 
             Assert.AreEqual("Error Message1",testScript.GetEmailFail("1234")); 
         } 
 
 
         [Test()] 
         public void ItWillAddDifferentChargeCodes()
         { 
             Script testScript = new Script("TestScript"); 
             testScript.AddEmailFail("1234", "Error Message4"); 
             testScript.AddEmailFail("1235", "Error Message2"); 
             Assert.AreEqual("Error Message4", testScript.GetEmailFail("1234")); 
             Assert.AreEqual("Error Message2", testScript.GetEmailFail("1235")); 
         } 

    }
}
