using System;
using OctagonCommon.Args;

namespace OctagonCommon
{
   public class Progresser
   {
      public static Progresser Instance;
      static Progresser()
      {
         Instance = new Progresser();
      }

      public event EventHandler<ProgresserArgs> ProgressChanged;

      private void OnProgressChanged(string eventName, double pct)
      {
         var handler = ProgressChanged;
         if (handler != null)
            handler(this, new ProgresserArgs(eventName, pct));
      }

      public event EventHandler<ProgresserEventArgs> EventStarted;
      private void OnEventStarted(string eventName, bool start)
      {
         var handler = EventStarted;
         if (handler != null)
            handler(this, new ProgresserEventArgs(eventName, start));
      }

      private double CurrentPct { get; set; }
      private string CurrentEventName { get; set; }

      private void DecideChangeProgressPct(string eventName, double pct)
      {
         if (!Equals(CurrentEventName, eventName) || Math.Abs(CurrentPct - pct) > 0.00025)
         {
            CurrentPct = pct;
            CurrentEventName = eventName;
            OnProgressChanged(CurrentEventName, CurrentPct);
         }
      }

      private double CurrentAdvance { get; set; }
      private void DecideChangeProgress(string eventName, int max)
      {
         if (!Equals(CurrentEventName, eventName))
         {
            CurrentPct = 0d;
            CurrentAdvance = 0;
            CurrentEventName = eventName;
            OnProgressChanged(CurrentEventName, CurrentPct);
         }
         else
         {
            CurrentAdvance += 1;
            var pct = CurrentAdvance / (double) max;
            if (Math.Abs(CurrentPct - pct) > 0.00025)
            {
               CurrentPct = pct;
               OnProgressChanged(CurrentEventName, CurrentPct);
            }
         }
      }

      public static void ChangeProgressPct(string eventName, double pct)
      {
         Instance.DecideChangeProgressPct(eventName, pct);
      }

      public static void ChangeProgress(string eventName, int max)
      {
         Instance.DecideChangeProgress(eventName, max);
      }

      public static void EventStart(string eventName)
      {
         Instance.OnEventStarted(eventName, true);
      }

      public static void EventEnd(string eventName)
      {
         Instance.OnEventStarted(eventName, false);                                          
      }
   }
}