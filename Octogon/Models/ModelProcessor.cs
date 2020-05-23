#region Dependencies

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using OctagonCommon;
using OctagonCommon.Args;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

#endregion

namespace Octagon.Models
{


   public class ModelProcessor : AModel
   {
      private readonly ListView _listViewLog;
      private bool _isIndeterminateProgress;
      private string _progressText;
      private double _progress;

      public ModelProcessor(Processor processor, ListView listViewLog)
      {
         Processor = processor;
         _listViewLog = listViewLog;
         LogEntrys = new ObservableCollection<ProcessorEntryLog>();
         Step1Entrys = new ObservableCollection<ProcessorStep1>();
         Step2Entrys = new ObservableCollection<ProcessorStep2>();
         Step3Entrys = new ObservableCollection<ProcessorStep3>();
         SearchResult = new ObservableCollection<SearchEntry>();
         //
         Logger.Instance.TextLogged += InstanceOnTextLogged;    
         Progresser.Instance.ProgressChanged += InstanceOnProgressChanged;
         Progresser.Instance.EventStarted += InstanceOnEventStarted;
         //
         Processor.ProcessorEnded += ProcessorOnProcessorEnded;
         //
         Dispatcher = Dispatcher.CurrentDispatcher;
         Stopwatch = new Stopwatch();
         StopwatchSecurity = new Stopwatch();
         //
         StopwatchTotal = new Stopwatch();
         DispatcherTimerUpdater = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.25) };
         DispatcherTimerUpdater.Tick += DispatcherTimerUpdaterOnTick;
         //
         Status = 0;
         ChangeBottomVisibility(TypeBottomVisibility.Progress);
      }

      private void DispatcherTimerUpdaterOnTick(object sender, EventArgs eventArgs)
      {
         var minutes = (int)StopwatchTotal.Elapsed.TotalMinutes;
         var fsec = 60 * (StopwatchTotal.Elapsed.TotalMinutes - minutes);
         var sec = (int)fsec;
         if (StopwatchTotal.IsRunning)
         {
            TotalTimeText = string.Format("Time elapsed {0}:{1:D2}", minutes, sec);
         }
         else
         {
            TotalTimeText = string.Format("Time elapsed (Stopped) {0}:{1:D2}", minutes, sec);
         }
         OnPropertyChanged("TotalTimeText");
      }

      public ObservableCollection<ProcessorStep1> Step1Entrys { get; set; }
      public ObservableCollection<ProcessorStep2> Step2Entrys { get; set; }
      public ObservableCollection<ProcessorStep3> Step3Entrys { get; set; }
      public ObservableCollection<SearchEntry> SearchResult { get; set; }

      public Dispatcher Dispatcher { get; set; }


      public ObservableCollection<ProcessorEntryLog> LogEntrys { get; set; }

      public Processor Processor { get; set; }

      public int Status { get; set; }

      public string TotalTimeText { get; set; }

      //public string StatusText { get; set; }

      public double Progress
      {
         get { return _progress; }
         set
         {
            _progress = value;
            OnPropertyChanged("Progress");
         }
      }

      public bool IsIndeterminateProgress
      {
         get { return _isIndeterminateProgress; }
         set
         {
            _isIndeterminateProgress = value;
            OnPropertyChanged("IsIndeterminateProgress");
         }
      }

      public string ProgressText
      {
         get { return _progressText; }
         set
         {
            _progressText = value;
            OnPropertyChanged("ProgressText");
         }
      }

      public Stopwatch Stopwatch { get; set; }
      public Stopwatch StopwatchTotal { get; set; }
      public Stopwatch StopwatchSecurity { get; set; }
      public DispatcherTimer DispatcherTimerUpdater { get; set; }

      public TypeBottomVisibility TypeVisibility { get; set; }

      public void ChangeBottomVisibility(TypeBottomVisibility typeBottomVisibility)
      {
         if (TypeVisibility != typeBottomVisibility)
         {
            TypeVisibility = typeBottomVisibility;
            OnPropertyChanged("VisibilityButtonClose");
            OnPropertyChanged("VisibilityProgressBar");
            OnPropertyChanged("VisibilityButtonContinue");
         }
      }


      public Visibility VisibilityButtonClose
      {
         get { return TypeVisibility == TypeBottomVisibility.Close ? Visibility.Visible : Visibility.Collapsed; }
      }

      public Visibility VisibilityProgressBar
      {
         get { return TypeVisibility == TypeBottomVisibility.Progress ? Visibility.Visible : Visibility.Collapsed; }
      }

      public Visibility VisibilityButtonContinue
      {
         get { return TypeVisibility == TypeBottomVisibility.Continue ? Visibility.Visible : Visibility.Collapsed; }
      }


      private void AddLog(ProcessorEntryLog le, bool forceScroll = false)
      {
         LogEntrys.Add(le);
         if (forceScroll || StopwatchSecurity.ElapsedMilliseconds > 100)
         {
            StopwatchSecurity.Restart();
            _listViewLog.ScrollIntoView(le);
            //
            if (LogEntrys.Count > 1000)
            {
               for (int i = 0; i < LogEntrys.Count - 1000; i++)
               {
                  LogEntrys.RemoveAt(i);
               }
            }
         }
      }
                         

      private void InstanceOnProgressChanged(object sender, ProgresserArgs progresserArgs)
      {
         Dispatcher.Invoke(new Action(() =>
         {
            Progress = Math.Max(0d, Math.Min(1d, progresserArgs.Pct));
            IsIndeterminateProgress = progresserArgs.Pct < -0.5;
            if (IsIndeterminateProgress)
            {
               ProgressText = string.Format("{0}...", progresserArgs.EventName);
            }
            else
            {
               ProgressText = string.Format("{0} {1:P1}", progresserArgs.EventName, Progress);
            }
         }));
      }


      private void InstanceOnEventStarted(object sender, ProgresserEventArgs progresserEventArgs)
      {
         Dispatcher.Invoke(new Action(() =>
         {
            if (progresserEventArgs.Start)
            {
               ProgressText = string.Format("{0}", progresserEventArgs.EventName);
               AddLog(new ProcessorEntryLog(string.Format("[{0} starts]", progresserEventArgs.EventName), TypeLog.Alert), true);
               IsIndeterminateProgress = true;
               Stopwatch.Restart();
               DispatcherTimerUpdater.Start();
               StopwatchTotal.Start();
            }
            else
            {
               Stopwatch.Stop();
               StopwatchTotal.Stop();
               DispatcherTimerUpdater.Stop();
               DispatcherTimerUpdaterOnTick(null, null);
               ProgressText = "Waiting next operation...";
               AddLog(new ProcessorEntryLog(string.Format("[{0} ended in {1:F2} seconds]", progresserEventArgs.EventName, Stopwatch.ElapsedMilliseconds / 1000d), TypeLog.Alert), true);
            }
         }));
      }

      private void InstanceOnTextLogged(object sender, LoggerArgs loggerArgs)
      {
         Dispatcher.Invoke(new Action(() => { AddLog(new ProcessorEntryLog(loggerArgs)); }));
      }

      private void ProcessorOnProcessorEnded(object sender, StatusArgs statusArgs)
      {
         Dispatcher.Invoke(new Action(() =>
         {
            //if (statusArgs.Status == -1)
            //{
            //   Stopwatch.Stop();
            //   StopwatchTotal.Stop();
            //   DispatcherTimerUpdater.Stop();
            //   DispatcherTimerUpdaterOnTick(null, null);
            //   AddLog(new ProcessorEntryLog(string.Format("[Ended with error(s) in {0:F2} seconds]", Stopwatch.ElapsedMilliseconds / 1000d), TypeLog.Error), true);
            //}
            //
            switch (Status)
            {   
               case 1:
                  FillStep1Results();
                  break;
               case 2:
                  FillStep2Results();
                  break;
               case 3:
                  FillStep3Results();
                  break;
               default:
                  break;
            }
            Status = statusArgs.Status;
            //
            if (Processor.ProcessorArgs.ConfigurationMain.IsNoConfirmationMessage)
            {
               ContinueProcess();
            }
            else
            {
               ChangeBottomVisibility(TypeBottomVisibility.Continue);
            }
         }));
      }

      private void ContinueProcess()
      {
         switch (Status)
         {
            case 1:
               Processor.StartProcess1();
               break;
            case 2:
               Processor.StartProcess2();
               break;
            case 3:
               Processor.StartProcess3();
               break;
            case -1:
               AddLog(new ProcessorEntryLog(string.Format("[Execution take a total of {0:F2} seconds]", StopwatchTotal.ElapsedMilliseconds / 1000d), TypeLog.Alert), true);
               ChangeBottomVisibility(TypeBottomVisibility.Close);
               break;

            default:
               throw new Exception("Bad ending status ?");
         }
      }

      public void FillStep1Results()
      {
         foreach (InformationFile fBsa in Processor.ProcessorArgs.FilesBsa)
         {
            Step1Entrys.Add(new ProcessorStep1(fBsa.FileSource.Name, fBsa.FileSource.FullName, fBsa.FileTarget.FullName, true));
         }
         foreach (InformationFile fDds in Processor.ProcessorArgs.FilesDds)
         {
            Step1Entrys.Add(new ProcessorStep1(fDds.FileSource.Name, fDds.FileSource.FullName, fDds.FileTarget.FullName, false));
         }
         OnPropertyChanged("VisibilityStep1Entrys");
      }

      public Visibility VisibilityStep1Entrys { get { return Step1Entrys.Any() ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityStep2Entrys { get { return Step2Entrys.Any() ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilityStep3Entrys { get { return Step3Entrys.Any() ? Visibility.Visible : Visibility.Collapsed; } }
      public Visibility VisibilitySearchResult { get { return SearchResult.Any() ? Visibility.Visible : Visibility.Collapsed; } }

      public void FillStep2Results()
      {
         foreach (InformationOrder order in Processor.ProcessorArgs.Orders)
         {
            Step2Entrys.Add(new ProcessorStep2(order, true));
         }
         foreach (InformationOrder order in Processor.ProcessorArgs.DiscardedOrders)
         {
            Step2Entrys.Add(new ProcessorStep2(order, false));
         }
         foreach (InformationCopy copy in Processor.ProcessorArgs.CopyOrders)
         {
            Step2Entrys.Add(new ProcessorStep2(copy, true));
         }
         //                            
         foreach (InformationOrder order in Processor.ProcessorArgs.SearchResults)
         {
            SearchResult.Add(new SearchEntry(order));
         }
         OnPropertyChanged("VisibilityStep2Entrys");
         OnPropertyChanged("VisibilitySearchResult");
      }

      public void FillStep3Results()
      {
         foreach (var folder in Processor.ProcessorArgs.Folders)
         {
            Step3Entrys.Add(new ProcessorStep3(folder));
         }
         OnPropertyChanged("VisibilityStep3Entrys");
      }

      public void Start()
      {
         StopwatchSecurity.Start();
         StopwatchTotal.Reset();
         Stopwatch.Reset();
         Processor.StartProcess0();
      }

      public void Continue()
      {
         ChangeBottomVisibility(TypeBottomVisibility.Progress);
         ContinueProcess();
      }

      public enum TypeBottomVisibility
      {
         Progress,
         Continue,
         Close
      }
   }
}