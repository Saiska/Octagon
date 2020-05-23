using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctagonCommon.Configurations;
using OctagonCommon.Statics;

namespace OctagonTesting
{
   [TestClass]
   public class UnitTestValidation
   {
      public TestContext TestContext { get; set; }

      private List<string> StringListForPattern;
      private List<string> StringListForOccurence;

      [TestInitialize]
      public void SelectionTestInitialization()
      {
         StringListForPattern = new List<string> { "Ag15", "Er35", "Yh98", "Ml67", "Ea89", "Yl78" };
         StringListForOccurence = new List<string> { "Akle.aze.eze.f4", "azeaz.kl.po.zez.fdfd.dfs", "dsf..sdf..sdfezf","ze.grege.reg" };
      }
         
      [TestMethod]
      public void SelectionTestExcludeOccurenceLess()
      {
         TestContext.WriteLine("SelectionTestExcludeOccurenceLess");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.ExcludeOccurenceLess, ".", 3);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForOccurence);
         Assert.IsTrue(result.Count == 3, "3 left");
      }
         
      [TestMethod]
      public void SelectionTestExcludeOccurenceEqualOrMore()
      {
         TestContext.WriteLine("SelectionTestExcludeOccurenceEqualOrMore");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.ExcludeOccurenceEqualOrMore, ".", 4);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForOccurence);
         Assert.IsTrue(result.Count == 2, "2 left");
      }
         
      [TestMethod]
      public void SelectionTestIncludeOccurenceEqualOrMore()
      {
         TestContext.WriteLine("SelectionTestIncludeOccurenceEqualOrMore");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.IncludeOccurenceEqualOrMore, ".", 5);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForOccurence);
         Assert.IsTrue(result.Count == 1, "1 left");
      }
         
      [TestMethod]
      public void SelectionTestIncludeOccurenceLess()
      {
         TestContext.WriteLine("SelectionTestIncludeOccurenceLess");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.IncludeOccurenceLess, ".", 4);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForOccurence);
         Assert.IsTrue(result.Count == 2, "2 left");
      }
            
      [TestMethod]
      public void SelectionTestAfterReset()
      {
         TestContext.WriteLine("SelectionTestAfterReset");
         var cs = new ConfigurationSelection();
         cs.RemoveFilter(cs.AddFilter(TypeSelection.Exclude, "g1",0));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.Include, "Ag", 0));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.ExcludeEnd, "35", 0));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.IncludeEnd, "89", 0));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.ExcludeStart, "Ml", 0));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.IncludeStart, "Y", 0));
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == StringListForPattern.Count, "All initial list recovered");
      }
                 
      [TestMethod]
      public void SelectionTestComplex1()
      {
         TestContext.WriteLine("SelectionTestComplex");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.Include, "7", 0);
         cs.AddFilter(TypeSelection.Exclude, "r", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == 2, "7 included / r excluded");
      }

      [TestMethod]
      public void SelectionTestComplex2()
      {
         TestContext.WriteLine("SelectionTestComplex");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.Exclude, "r", 0);
         cs.AddFilter(TypeSelection.ExcludeEnd, "9", 0);
         cs.AddFilter(TypeSelection.ExcludeStart, "y", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == 2, "r,9,y excluded");
      }

      [TestMethod]
      public void SelectionTestInclude()
      {
         TestContext.WriteLine("SelectionTestInclude");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.Include, "7", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == 2, "7 included");
      }

      [TestMethod]
      public void SelectionTestIncludeEnd()
      {
         TestContext.WriteLine("SelectionTestIncludeEnd");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.IncludeEnd, "5", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == 2, "5 included");
      }

      [TestMethod]
      public void SelectionTestIncludeStart()
      {
         TestContext.WriteLine("SelectionTestIncludeStart");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.IncludeStart, "e", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == 2, "e included");
      }

      [TestMethod]
      public void SelectionTestExclude()
      {
         TestContext.WriteLine("SelectionTestExclude");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.Exclude, "G1", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == StringListForPattern.Count - 1, "G1 excluded");
      }

      [TestMethod]
      public void SelectionTestExcludeStart()
      {
         TestContext.WriteLine("SelectionTestExcludeStart");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.ExcludeStart, "ag", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == StringListForPattern.Count - 1, "ag excluded");
      }

      [TestMethod]
      public void SelectionTestExcludeEnd()
      {
         TestContext.WriteLine("SelectionTestExcludeEnd");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.ExcludeEnd, "9", 0);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == StringListForPattern.Count - 1, "9 excluded");
      }

      private List<string> Validate(ConfigurationSelection cs, List<string> list)
      {
         var result = list.Where(cs.GetValidation).ToList();
         TestContext.WriteLine("Result:");
         foreach (var str in result)
         {
            TestContext.WriteLine("{0}", str);
         }
         return result;
      }
   }
}
