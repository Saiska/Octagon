using System.Collections.Generic;

namespace OctagonCommon.Statics
{
   public static class TextureExtensionList
   {
      public static List<string> List;

      static TextureExtensionList()
      {
         List = new List<string>
         {
            ".DDS"
         };
      }
   }
}