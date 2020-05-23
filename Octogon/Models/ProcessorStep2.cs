using System.IO;
using System.Linq;
using OctagonCommon.Informations;

namespace Octagon.Models
{
   public class ProcessorStep2
   {
      public bool ConfirmedOrder { get; private set; }

      public string FileSourceName { get; private set; }
      public string FileSourceFullName { get; private set; }
      public string FileTargetFullName { get; private set; }

      public string Options { get; private set; }

      public string TextureInfoOriginal { get; private set; }
      public string TextureInfoTarget { get; private set; }


      public ProcessorStep2(InformationCopy order, bool confirmedOrder)
      {
         ConfirmedOrder = confirmedOrder;
         //
         Options = "Copy into merge dir";
         //                             
         FileTargetFullName = order.Target;
         if (order.FileSource != null)
         {
            FileSourceName = order.FileSource.Name;
            FileSourceFullName = order.FileSource.FullName;
            //                                        
            if (order.FileSource.Exists)
            {
               TextureInfoOriginal = string.Format("{0} Mo", order.FileSource.Length / 1000000d);
            }
         }
         else
         {
            FileSourceName = "Deletion";
         }
         //
         var fTarget = new FileInfo(order.Target);
         if (fTarget.Exists)
         {
            TextureInfoTarget = string.Format("{0} Mo", fTarget.Length / 1000000d);
         }
      }

      public ProcessorStep2(InformationOrder order, bool confirmedOrder)
      {
         ConfirmedOrder = confirmedOrder;
         FileSourceName = order.FileSource.Name;
         FileSourceFullName = order.FileSource.FullName;
         FileTargetFullName = order.FileTarget.FullName;
         //
         var bools = new[]
         {
            order.IsBsaFormatCompressed?"Compressed Bsa":"", 
            order.IsDoTexconv?"Do Texconv":"", 
            order.IsGmicPass?"Do Gmic":"",
            order.IsRecopyOriginal?"Restore":"", 
            order.IsRefreshBackup?"Backup":"", 
            order.IsUseBackup?"Use Backup":""
         };
         Options = bools.Aggregate((a, b) => string.IsNullOrEmpty(a) ? b : string.IsNullOrEmpty(b) ? a : string.Format("{0},{1}", a, b));
         //                        
         if (order.OriginalSize != null)
         {
            TextureInfoOriginal = GetTextureInfo(order.OriginalSize);
         }
         if (order.TargetSize != null)
         {
            TextureInfoTarget = GetTextureInfoComplete(order.TargetSize);
         }
      }

      private static string GetTextureInfo(InformationImage info)
      {
         if (info == null)
            return "Not needed";
         return string.Format("{0}/{1}px mipmaps={2} format={3}", info.Width, info.Height, info.Mipmaps, info.Format);
      }

      private static string GetTextureInfoComplete(InformationImage info)
      {
         if (info == null)
            return "Not needed";
         return string.Format("{0} compression={1}", GetTextureInfo(info), info.TypeTexCompression);
      }
   }
}