namespace Octagon.Models
{
   public class ProxyBsaGame
   {
      public string BsaGame { get; set; }
      public string Text { get; set; }

      public ProxyBsaGame(string bsaGame, string text)
      {
         BsaGame = bsaGame;
         Text = text;        
      }
   }
}