using System.IO;

namespace OctagonCommon.Informations
{
   public class InformationCopy
   {
      public InformationCopy(FileInfo fileSource, string target, bool confirmed, bool renamed)
      {
         FileSource = fileSource;
         Target = target;
         Confirmed = confirmed || renamed;
         Renamed = renamed;
      }

      public FileInfo FileSource { get; private set; }
      public string Target { get; private set; }
      public bool Confirmed { get; private set; }
      public bool Renamed { get; private set; }

      public void ChangeFileSource(FileInfo fileSource, bool confirmed, bool renamed)
      {
         FileSource = fileSource;
         Confirmed = confirmed || renamed;
         Renamed = renamed;
      }
   }
}