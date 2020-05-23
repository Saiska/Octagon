using System.Collections.Generic;
using OctagonCommon.Configurations;
using OctagonCommon.Informations;

namespace OctagonCommon.Args
{
   public class ProcessorArgs
   {
      public ConfigurationMain ConfigurationMain { get; set; }

      public List<InformationOrder> Orders { get; set; }

      public List<InformationOrder> DiscardedOrders { get; set; }

      public List<InformationOrder> SearchResults { get; set; }

      public List<InformationOrder> BsaOrders { get; set; }

      public List<InformationCopy> CopyOrders { get; set; }

      public List<InformationFileDeletion> DeleteOrders { get; set; }

      public ProcessorArgs(ConfigurationMain configurationMain)
      {
         ConfigurationMain = configurationMain;
         Folders = new Dictionary<string, InformationDirectory>();
         FilesDds = new List<InformationFile>();
         FilesBsa = new List<InformationFile>();
         Orders = new List<InformationOrder>();
         DiscardedOrders = new List<InformationOrder>();
         SearchResults = new List<InformationOrder>();
         BsaOrders = new List<InformationOrder>();
         CopyOrders = new List<InformationCopy>();
         DeleteOrders = new List<InformationFileDeletion>();
      }

      public Dictionary<string, InformationDirectory> Folders;

      public List<InformationFile> FilesDds;

      public List<InformationFile> FilesBsa;
   }
}