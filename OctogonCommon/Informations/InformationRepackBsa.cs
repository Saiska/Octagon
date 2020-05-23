using System.IO;

namespace OctagonCommon.Informations
{
   public class InformationRepackBsa
   {
      public DirectoryInfo Source { get; private set; }
      public string CurrentBsaName { get; private set; }
      public DirectoryInfo CurrentTarget { get; private set; }
      public bool IsCompressed { get; set; }
      public string GameParameter { get; set; }

      public InformationRepackBsa(DirectoryInfo source, string currentBsaName, string gameParameter, DirectoryInfo currentTarget, bool isCompressed)
      {
         Source = source;
         CurrentBsaName = currentBsaName;
         CurrentTarget = currentTarget;
         IsCompressed = isCompressed;
         GameParameter = gameParameter;
      }
   }
}