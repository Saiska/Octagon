#region Dependencies

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OctagonCommon.Configurations;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

#endregion

namespace OctagonCommon.Executions
{
   public class FileExecution
   {
      private const string ProgressExecuteOrderTextures = "Processing textures";
      private const string ProgressExecuteOrderArchives = "Processing archives";
      private const string ProgressCleaningBsa = "Cleaning archives";
      private const string ProgressCopyBsaAsLoose = "Transform archive into loose files";
      private const string ProgressMerge = "Merging Mods into single directory";

      private const string ProgressUnpacking = "Unpacking archives";
      private const string ProgressRepacking = "Repacking archives";

      public ExternalTools ExternalTools { get; set; }

      public FileExecution(ExternalTools externalTools)
      {
         ExternalTools = externalTools;
      }

      public void CopyBsaAsLooseSimplified(List<InformationOrder> bsaOrders, ConfigurationMain mainCfg)
      {
         //                             
         Progresser.EventStart(ProgressUnpacking);
         //   
         foreach (var order in bsaOrders)
         {
            Progresser.ChangeProgress(ProgressUnpacking, bsaOrders.Count);
            //
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Unpacking {0}", order.FileSource.Name);
            }
            //
            if (order.FileSource.Directory == null)
            {
               Logger.Log("Bsa unpacking bad directory for {0}", order.FileSource.FullName, TypeLog.Error);
               continue;
            }
            //                                          
            var dir = order.FileSource.Directory;
            //      
            try
            {
               ExternalTools.CallBsaUnPack(order.FileSource.FullName, dir.FullName, mainCfg.IsUseMultithreading, mainCfg.IsVerbose);
               //
               if (mainCfg.IsVerbose)
               {
                  Logger.Log("Deleting BSA {0}", order.FileSource.Name);
               }
               order.FileSource.Delete();
            }
            catch (Exception e)
            {
               Logger.Log(e);
            }
            //
         }
         //                   
         Logger.Log("Unpacking BSA complete. {0} unpacking done ", bsaOrders.Count);
         Progresser.EventEnd(ProgressUnpacking);
      }

      public void CopyBsaAsLoose(List<InformationOrder> bsaOrders, ConfigurationMain mainCfg)
      {
         Progresser.EventStart(ProgressCopyBsaAsLoose);
         //   
         foreach (var order in bsaOrders)
         {
            Progresser.ChangeProgress(ProgressCopyBsaAsLoose, bsaOrders.Count);
            //        
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Copying BSA as loose files {0}", order.FileSource.Name);
            }
            //
            var dirPath = FileUtils.GetBsaTempDirectory(order.FileSource);
            var bsaDir = new DirectoryInfo(dirPath);
            if (!bsaDir.Exists)
            {
               throw new DirectoryNotFoundException(string.Format("BSA temp directory don't exist for unknow reason, unpacking seems to have fails. Canceling copy and deletion of bsa file: {0}", dirPath));
            }
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Copying as loose files: {0}", order.FileSource.Name);
            }
            Dictionary<long, InformationCopy> copies = new Dictionary<long, InformationCopy>();
            FileUtils.CopyDirContent(bsaDir, order.FileSource.Directory, copies, mainCfg.IsMergeAssertCase);
            string title = string.Format("{0}: {1}", ProgressCopyBsaAsLoose, order.FileSource.Name);
            foreach (InformationCopy informationCopy in copies.Values)
            {
               Progresser.ChangeProgress(title, copies.Count);
               FileUtils.ExecuteMove(informationCopy);
            }
            //
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Deleting BSA {0}", order.FileSource.Name);
            }
            //
            order.FileSource.Delete();
         }
         //   
         Logger.Log("Copy BSA as loose files complete. {0} archives done ", bsaOrders.Count);
         Progresser.EventEnd(ProgressCopyBsaAsLoose);
      }

      public void CleanBsa(List<InformationOrder> bsaOrders, bool verbose)
      {
         Progresser.EventStart(ProgressCleaningBsa);
         //   
         foreach (var order in bsaOrders)
         {
            Progresser.ChangeProgress(ProgressCleaningBsa, bsaOrders.Count);
            //
            if (verbose)
            {
               Logger.Log("Cleaning Temp BSA {0}", order.FileSource.Name);
            }
            //
            var dirPath = FileUtils.GetBsaTempDirectory(order.FileSource);
            //
            //Directory.Delete(dirPath, true);
            FileUtils.DeleteCompleteDirectory(new DirectoryInfo(dirPath));
         }
         //   
         Logger.Log("Cleaning Temp BSA complete. {0} cleaning done ", bsaOrders.Count);
         Progresser.EventEnd(ProgressCleaningBsa);
      }

      internal void ExecuteTreat(List<InformationOrder> orders, ConfigurationMain mainCfg, bool isBsa)
      {
         var eventName = isBsa ? ProgressExecuteOrderArchives : ProgressExecuteOrderTextures;
         Progresser.EventStart(eventName);
         //    
         if (mainCfg.IsUseMultithreading)
         {
            Parallel.ForEach(orders, order =>
            {
               ExecuteOrder(orders, mainCfg, order, eventName);
            });
         }
         else
         {
            foreach (var order in orders)
            {
               ExecuteOrder(orders, mainCfg, order, eventName);
            }
         }
         //                
         Logger.Log("Files processing complete. {0} operations done ", orders.Count);
         Progresser.EventEnd(eventName);
      }

      private void ExecuteOrder(List<InformationOrder> orders, ConfigurationMain mainCfg, InformationOrder order, string eventName)
      {
         Progresser.ChangeProgress(eventName, orders.Count);
         //
         if (mainCfg.IsVerbose)
         {
            Logger.Log(string.Format("Processing {0}", order.FileSource.FullName));
         }
         //
         if (order.IsRefreshBackup && order.FileTarget != null)
         {
            order.FileSource.CopyTo(order.FileTarget.FullName, true);
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Refreshing backup {0}=>{1}", order.FileSource.FullName, order.FileTarget.FullName);
            }
         }
         //
         if (order.IsRecopyOriginal && order.FileTarget != null)
         {
            order.FileTarget.CopyTo(order.FileSource.FullName, true);
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Restoring original file {0}=>{1}", order.FileTarget.FullName, order.FileSource.FullName);
            }
         }
         //
         if (order.IsGmicPass)
         {
            if (order.FileSource.Directory != null)
            {
               ExternalTools.CallTexConvDdsToPng(order.FileSource.FullName, order.FileSource.Directory.FullName);
               var newFilePath = order.FileSource.FullName.Substring(0, order.FileSource.FullName.Length - 4) + ".png";
               foreach (var orderGmicCommand in order.GmicCommands)
               {
                  ExternalTools.CallGmic(newFilePath, orderGmicCommand);
               }
               CreateDDS(mainCfg, newFilePath, order);
               File.Delete(newFilePath);
            }
         }
         //
         if (order.IsCustomPass)
         {
            if (order.FileSource.Directory != null)
            {
               string pathSource = order.FileSource.FullName;
               if (order.IsApplyOnPng)
               {
                  ExternalTools.CallTexConvDdsToPng(order.FileSource.FullName, order.FileSource.Directory.FullName);
                  pathSource = order.FileSource.FullName.Substring(0, order.FileSource.FullName.Length - 4) + ".png";
               }
               //
               FileInfo file = new FileInfo(pathSource);
               foreach (var orderCustomCommand in order.CustomCommands)
               {
                  string commandWithFile = orderCustomCommand
                     .Replace("%F%", file.FullName)
                     .Replace("%N%", file.Name)
                     .Replace("%E%", file.Extension);
                  ExternalTools.CallCustom(commandWithFile);
               }
               //
               if (order.IsApplyOnPng)
               {
                  CreateDDS(mainCfg, pathSource, order);
                  File.Delete(pathSource);
               }
            }
         }
         //    
         if (order.IsDoTexconv && order.TargetSize != null)
         {
            if (order.FileSource.Directory != null)
            {
               var textureSource = order.IsUseBackup && order.FileTarget != null ? order.FileTarget.FullName : order.FileSource.FullName;
               //
               CreateDDS(mainCfg, textureSource, order);
            }
         }
      }

      private void CreateDDS(ConfigurationMain mainCfg, string textureSource, InformationOrder order)
      {
         var tTexConv = ExternalTools.CallTexConv(textureSource, order.FileSource.Directory.FullName, order.TargetSize.Width,
            order.TargetSize.Height, Math.Max(order.TargetSize.Mipmaps, 1), order.TargetSize.Format, order.TargetSize.TypeTexCompression);
         //
         if (tTexConv.IndexOf("FAILED", StringComparison.OrdinalIgnoreCase) >= 0)
         {
            Logger.Log("Failed to process {0} with parameters w={1} h={2} m={3}", order.FileSource.FullName, order.TargetSize.Width, order.TargetSize.Height,
               order.TargetSize.Mipmaps, TypeLog.Error);
         }
         else
         {
            if (mainCfg.IsVerbose)
            {
               Logger.Log(string.Format("Processed to: w={0} h={1} m={2}", order.TargetSize.Width, order.TargetSize.Height,
                  order.TargetSize.Mipmaps));
            }
         }
      }




      public void RepackBsa(List<InformationOrder> bsaOrders, string bsarchGameParameter, bool isMultithread, bool verbose)
      {
         //                               
         Progresser.EventStart(ProgressRepacking);
         //
         foreach (var order in bsaOrders)
         {
            Progresser.ChangeProgress(ProgressRepacking, bsaOrders.Count);
            //
            if (verbose)
            {
               Logger.Log("Repacking {0}", order.FileSource.Name);
            }
            // 
            var dirPath = FileUtils.GetBsaTempDirectory(order.FileSource);
            var dir = new DirectoryInfo(dirPath);
            //
            if (dir.Exists)
            {
               var res = ExternalTools.CallBsaPack(order.FileSource.FullName, dir.FullName, order.IsBsaFormatCompressed, bsarchGameParameter, isMultithread, verbose);
               if (verbose)
               {
                  foreach (var re in res)
                  {
                     Logger.Log(re);
                  }
               }
            }
         }
         //                
         Logger.Log("Repacking BSA complete. {0} packing done ", bsaOrders.Count);
         Progresser.EventEnd(ProgressRepacking);
      }

      public void UnpackBsa(List<InformationOrder> bsaOrders, ConfigurationMain mainCfg)
      {
         //                             
         Progresser.EventStart(ProgressUnpacking);
         //   
         foreach (var order in bsaOrders)
         {
            Progresser.ChangeProgress(ProgressUnpacking, bsaOrders.Count);
            //
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Unpacking {0}", order.FileSource.Name);
            }
            //
            if (order.FileSource.Directory == null)
            {
               Logger.Log("Bsa unpacking error for {0} unrecognized file source directory", order.FileSource.FullName, TypeLog.Error);
               continue;
            }
            //
            var dirPath = FileUtils.GetBsaTempDirectory(order.FileSource);
            var dir = new DirectoryInfo(dirPath);
            //
            if (!dir.Exists)
            {
               dir.Create();
               //    
               try
               {
                  ExternalTools.CallBsaUnPack(order.FileSource.FullName, dir.FullName, mainCfg.IsUseMultithreading, mainCfg.IsVerbose);
               }
               catch (Exception e)
               {
                  Logger.Log(e);
               }
            }
         }
         //                   
         Logger.Log("Unpacking BSA complete. {0} unpacking done ", bsaOrders.Count);
         Progresser.EventEnd(ProgressUnpacking);
      }

      public void Merge(ConfigurationMain mainCfg, List<InformationCopy> copies)
      {
         Progresser.EventStart(ProgressMerge);
         //       
         foreach (InformationCopy informationCopy in copies)
         {
            Progresser.ChangeProgress(ProgressMerge, copies.Count);
            FileUtils.ExecuteCopyOrDelete(informationCopy);
         }
         //   
         Logger.Log("Merge complete. {0} files copied ", copies.Count);
         Progresser.EventEnd(ProgressMerge);
      }

      public void DeleteFiles(ConfigurationMain mainCfg, List<InformationFileDeletion> deletes)
      {
         Progresser.EventStart(ProgressMerge);
         //       
         foreach (InformationFileDeletion informationFileDeletion in deletes)
         {
            Progresser.ChangeProgress(ProgressMerge, deletes.Count);
            informationFileDeletion.FileToDelete.Refresh();
            if (informationFileDeletion.FileToDelete.Exists)
            {
               informationFileDeletion.FileToDelete.Delete();
            }
         }
         //   
         Logger.Log("Merge complete. {0} files copied ", deletes.Count);
         Progresser.EventEnd(ProgressMerge);
      }
   }
}