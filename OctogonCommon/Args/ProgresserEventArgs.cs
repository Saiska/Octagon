using System;

namespace OctagonCommon.Args
{
   public class ProgresserEventArgs : EventArgs
   {
      public bool Start { get; set; }
      public string EventName { get; set; }

      public ProgresserEventArgs(string eventName, bool start)
      {
         Start = start;
         EventName = eventName;
      }
   }
}