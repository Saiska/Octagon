#region Dependencies

using System.Collections.Generic;
using System.IO;
using System.Linq;

#endregion

namespace OctagonCommon.Informations
{
   public class InformationFile
   {
      public InformationFile(FileInfo fileSource, FileInfo fileTarget)
      {
         FileSource = fileSource;
         FileTarget = fileTarget;
      }

      public FileInfo FileSource { get; private set; }
      public FileInfo FileTarget { get; private set; }
   }
}