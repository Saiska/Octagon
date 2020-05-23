namespace OctagonCommon.Informations
{
   public class InformationPriority
   {
      public InformationPriority(int priority, string modName)
      {
         Priority = priority;
         ModName = modName;
      }

      public int Priority { get; private set; }
      public string ModName { get; private set; }
   }
}