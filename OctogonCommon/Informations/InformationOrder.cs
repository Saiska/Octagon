#region Dependencies

using System.Collections.Generic;
using System.IO;

#endregion

namespace OctagonCommon.Informations
{
   public class InformationOrder
   {
      public InformationOrder(FileInfo fileSource, FileInfo fileTarget)
      {
         FileSource = fileSource;
         FileTarget = fileTarget;
         GmicCommands = new List<string>();
         CustomCommands = new List<string>();
      }                                       

      public FileInfo FileSource { get; private set; }
      public FileInfo FileTarget { get; private set; }

      public bool IsBsaFormatCompressed { get; set; }   
      public bool IsDoTexconv { get; set; }   
      public bool IsGmicPass { get; set; }
      public bool IsCustomPass { get; set; }    
      public bool IsRecopyOriginal { get; set; }  
      public bool IsRefreshBackup { get; set; }
      public bool IsUseBackup { get; set; }
      public bool IsApplyOnPng { get; set; }

      public InformationImage TargetSize { get; set; }
      public InformationImage OriginalSize { get; set; }

      public List<string> GmicCommands { get; set; }
      public List<string> CustomCommands { get; set; }
   }
}