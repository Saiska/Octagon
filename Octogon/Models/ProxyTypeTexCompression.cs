using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class ProxyTypeTexCompression
   {
      public TypeTexCompression TypeTexCompression { get; set; }
      public string Text { get; set; }

      public ProxyTypeTexCompression(TypeTexCompression typeTexCompression, string text)
      {
         TypeTexCompression = typeTexCompression;
         Text = text;
      }
   }
}