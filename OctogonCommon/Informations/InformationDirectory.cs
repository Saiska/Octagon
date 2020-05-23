#region Dependencies

using System.IO;

#endregion

namespace OctagonCommon.Informations
{
   public class InformationDirectory
   {
      public DirectoryInfo DirectoryInfo { get; set; }

      public double NewSize { get; set; }

      public double Size { get; set; }
   }
}