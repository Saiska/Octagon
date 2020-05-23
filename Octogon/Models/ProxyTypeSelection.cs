using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class ProxyTypeSelection
   {
      public TypeSelection TypeSelection { get; set; }
      public string Text { get; set; }

      public ProxyTypeSelection(TypeSelection typeSelection, string text)
      {
         TypeSelection = typeSelection;
         Text = text;
      }
   }
}