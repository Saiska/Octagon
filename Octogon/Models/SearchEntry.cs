using OctagonCommon.Args;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class SearchEntry
   {
      public SearchEntry(InformationOrder order)
      {
         Name = order.FileSource.Name;
         FullName = order.FileSource.FullName;
         if (order.OriginalSize != null)
         {
            Format = order.OriginalSize.Format;
            Width = order.OriginalSize.Width;
            Height = order.OriginalSize.Height;
            IsMipmaps = order.OriginalSize.Mipmaps > 1;
         }
      }

      public bool IsMipmaps { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
      public string Name { get; set; }
      public string FullName { get; set; }
      public string Format { get; set; }

   }
}