using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctagonTesting
{
   [TestClass]
   public class UnitTest2
   {

      //A NE SURTOUT POAS ACTIVER SI HESITATION
      //[TestMethod]
      //public void TestMethod1()
      //{
      //   DirectoryInfo source = new DirectoryInfo(@"G:\skyrimtests\SkyrimMods\mods");
      //   DirectoryInfo target = new DirectoryInfo(@"G:\SkyrimMods\mods");
      //   //
      //   var subDirsS = source.GetDirectories();
      //   var subDirsT = target.GetDirectories();
      //   //
      //   foreach (DirectoryInfo directoryInfo in subDirsT)
      //   {
      //      if (subDirsS.Any(s => string.Equals(s.Name, directoryInfo.Name, StringComparison.CurrentCultureIgnoreCase)))
      //      {
      //         while (true)
      //         {
      //            try
      //            {
      //               directoryInfo.Delete(true);
      //            }
      //            catch
      //            {
      //               continue;
      //            }
      //            break;
      //         }
      //         directoryInfo.Refresh();
      //         Assert.IsFalse(directoryInfo.Exists);
      //      }
      //   }
      //}
   }
}
