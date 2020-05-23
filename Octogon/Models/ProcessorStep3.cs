using System.Collections.Generic;
using OctagonCommon.Informations;

namespace Octagon.Models
{
   public class ProcessorStep3
   {
      public string Directory { get; set; }
      public double Size { get; set; }
      public double NewSize { get; set; }

      public ProcessorStep3(KeyValuePair<string, InformationDirectory> folder)
      {
         Directory = folder.Key;
         Size = folder.Value.Size;
         NewSize = folder.Value.NewSize;
      }
   }
}