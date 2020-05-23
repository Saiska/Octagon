using System;

namespace OctagonCommon.Args
{
   public class StatusArgs : EventArgs
   {
      public int Status { get; set; }

      public StatusArgs(int status)
      {
         Status = status;
      }
   }
}