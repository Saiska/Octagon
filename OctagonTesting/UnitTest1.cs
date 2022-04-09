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

      private List<Tuple<string, string>> StringListForPattern;
      private List<Tuple<string, string>> StringListForOccurence;

      [TestInitialize]
      public void SelectionTestInitialization()
      {
         StringListForPattern = new List<Tuple<string, string>>
         {
            new Tuple<string,string>("Ag15",@"c:\temp\test\textures\Ag15.dds"), 
            new Tuple<string,string>("Er35",@"c:\temp\test\textures\Er35.dds"), 
            new Tuple<string,string>("Yh98",@"c:\temp\other\textures\Yh98.dds"), 
            new Tuple<string,string>("Ml67",@"c:\temp\other\textures\Ml67.dds"), 
            new Tuple<string,string>("Ea89",@"c:\temp\other\textures\Ea89.dds"), 
            new Tuple<string,string>("Yl78",@"c:\temp\other\textures\Yl78.dds"),          
         };
         StringListForOccurence = new List<Tuple<string, string>>
         {
            new Tuple<string,string>("Akle.aze.eze.f4",@"c:\temp\test\textures\Akle.aze.eze.f4"), 
            new Tuple<string,string>("azeaz.kl.po.zez.fdfd.dfs",@"c:\temp\test\textures\azeaz.kl.po.zez.fdfd.dfs"), 
            new Tuple<string,string>("dsf..sdf..sdfezf",@"c:\temp\test\textures\dsf..sdf..sdfezf"),
            new Tuple<string,string>( "ze.grege.reg",@"c:\temp\test\textures\ze.grege.reg")
         };
      }

      [TestMethod]
      public void SelectionTestExcludeOccurenceLess()
      {
         TestContext.WriteLine("SelectionTestExcludeOccurenceLess");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.ExcludeOccurenceLess, ".", 3, false);
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
         cs.AddFilter(TypeSelection.ExcludeOccurenceEqualOrMore, ".", 4, false);
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
         cs.AddFilter(TypeSelection.IncludeOccurenceEqualOrMore, ".", 5, false);
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
         cs.AddFilter(TypeSelection.IncludeOccurenceLess, ".", 4, false);
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
         cs.RemoveFilter(cs.AddFilter(TypeSelection.Exclude, "g1", 0, false));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.Include, "Ag", 0, false));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.ExcludeEnd, "35", 0, false));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.IncludeEnd, "89", 0, false));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.ExcludeStart, "Ml", 0, false));
         cs.RemoveFilter(cs.AddFilter(TypeSelection.IncludeStart, "Y", 0, false));
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
         cs.AddFilter(TypeSelection.Include, "7", 0, false);
         cs.AddFilter(TypeSelection.Exclude, "r", 0, false);
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
         cs.AddFilter(TypeSelection.Exclude, "r", 0, false);
         cs.AddFilter(TypeSelection.ExcludeEnd, "9", 0, false);
         cs.AddFilter(TypeSelection.ExcludeStart, "y", 0, false);
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
         cs.AddFilter(TypeSelection.Include, "7", 0, false);
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
         cs.AddFilter(TypeSelection.IncludeEnd, "5", 0, false);
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
         cs.AddFilter(TypeSelection.IncludeStart, "e", 0, false);
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
         cs.AddFilter(TypeSelection.Exclude, "G1", 0, false);
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
         cs.AddFilter(TypeSelection.ExcludeStart, "ag", 0, false);
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
         cs.AddFilter(TypeSelection.ExcludeEnd, "9", 0, false);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == StringListForPattern.Count - 1, "9 excluded");
      }

      [TestMethod]
      public void SelectionTestPath()
      {
         TestContext.WriteLine("SelectionTestExcludeEnd");
         var cs = new ConfigurationSelection();
         cs.AddFilter(TypeSelection.Include, @"\test\", 0, true);
         cs.CalculateStartFileValidation();
         //
         var result = Validate(cs, StringListForPattern);
         Assert.IsTrue(result.Count == StringListForPattern.Count - 4, "2 path included");
      }

      private List<Tuple<string, string>> Validate(ConfigurationSelection cs, List<Tuple<string, string>> list)
      {
         var result = list.Where(e => cs.GetValidation(e.Item1, e.Item2)).ToList();
         TestContext.WriteLine("Result:");
         foreach (var str in result)
         {
            TestContext.WriteLine("{0}", str);
         }
         return result;
      }
   }
}
