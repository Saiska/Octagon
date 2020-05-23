using System.Collections.Generic;

namespace OctagonCommon.Statics
{
   public static class ArchiveExtensionList
   {
      public static List<string> List;

      static ArchiveExtensionList()
      {
         List = new List<string>
         {
            ".BSA",
            ".BA2"
         };
      }
   }
}