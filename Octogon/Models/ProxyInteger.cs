namespace Octagon.Models
{
   public class ProxyInteger
   {
      public int Integer { get; set; }
      public string Text { get; set; }

      public ProxyInteger(int integer, string text)
      {
         Integer = integer;
         Text = text;        
      }
   }
}