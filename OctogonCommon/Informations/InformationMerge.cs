using System.IO;

namespace OctagonCommon.Informations
{
   public class InformationMerge
   {
      public InformationMerge(DirectoryInfo dirSource, DirectoryInfo dirTarget)
      {
         DirSource = dirSource;
         DirTarget = dirTarget;
      }

      public DirectoryInfo DirSource { get; private set; }
      public DirectoryInfo DirTarget { get; private set; }
   }
}