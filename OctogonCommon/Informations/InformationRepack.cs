using System.Collections.Generic;
using System.IO;
using OctagonCommon.Configurations;

namespace OctagonCommon.Informations
{
   public class InformationRepack
   {      
      public InformationRepack(ConfigurationRepack repack)
      {
         ConfigurationRepack = repack;
         Sources = new List<DirectoryInfo>();
      }


      public ConfigurationRepack ConfigurationRepack { get; set; }
      public List<DirectoryInfo> Sources { get; set; }
   }
}