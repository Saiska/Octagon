using System.Collections.Generic;

namespace OctagonCommon.Statics
{
   public static class GameParameterExtensionList
   {
      public static List<GameParameter> List;
      public static GameParameter Sse;
      public static GameParameter Fo4;
      public static GameParameter Fo4dds;

      static GameParameterExtensionList()
      {
         Sse = new GameParameter("sse", "Skyrim Special Edition archive format ");
         Fo4 = new GameParameter("fo4", "Fallout 4 General archive format ");
         Fo4dds = new GameParameter("fo4dds", "Fallout 4 DDS archive format (streamed DDS textures mipmaps) ");

         List = new List<GameParameter>
         {
            Sse,Fo4  ,Fo4dds
         };
      }

      public class GameParameter
      {

         public string Parameter;
         public string GameName;

         public GameParameter(string parameter, string gameName)
         {
            Parameter = parameter;
            GameName = gameName;
         }
      }
   }
}