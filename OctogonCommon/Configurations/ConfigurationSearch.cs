using System;
using System.Linq;
using OctagonCommon.Statics;

namespace OctagonCommon.Configurations
{
   public class ConfigurationSearch
   {
      public bool IsSearchEnabled { get; set; }
      public bool IsApplySearchToProcess { get; set; }

      public bool IsSearchNameEnabled { get; set; }
      public bool IsSearchMinSizeEnabled { get; set; }
      public bool IsSearchMaxSizeEnabled { get; set; }
      public bool IsSearchFormatEnabled { get; set; }
      public bool IsSearchMipmapsEnabled { get; set; }
      public bool IsSearchPowerOf2 { get; set; }

      public int MinSize { get; set; }
      public int MaxSize { get; set; }
      public bool IsMipmaps { get; set; }
      public bool IsPowerOf2 { get; set; }
      public string Name { get; set; }
      public string Format { get; set; }

      public ConfigurationSearch()
      {
         Name = string.Empty;
         MinSize = 1024;
         MaxSize = 1024;
         Format = DxgiFormatList.Formats.First();
      }

      /// <summary>
      /// Get if a search in depth from the file properties (from dxdiag) is NOT needed,
      /// or we can just know from the file name if any more work is needed
      /// </summary>
      /// <param name="fileSourceName"></param>
      /// <returns></returns>
      public bool IsSearchInDepthNotNeeded(string fileSourceName)
      {
         if (IsSearchEnabled && IsSearchNameEnabled && IsApplySearchToProcess && IsSearchNameEnabled)
         {
            return fileSourceName.IndexOf(Name, StringComparison.OrdinalIgnoreCase) < 0;
         }
         return false;
      }
   }
}