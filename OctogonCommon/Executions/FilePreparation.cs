#region Dependencies

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OctagonCommon.Configurations;
using OctagonCommon.Informations;
using OctagonCommon.Statics;

#endregion

namespace OctagonCommon.Executions
{
   public class FilePreparation
   {
      private const string ProgressAnalyzingTextures = "Analyzing textures";
      private const string ProgressAnalyzingArchives = "Analyzing archives";
      private const string ProgressListingMergingOp = "Listing merging operation";
      private const string ProgressMergeDeleteIfNotInSource = "Searching missing files in source";

      public ExternalTools ExternalTools { get; set; }

      public FilePreparation(ExternalTools externalTools)
      {
         ExternalTools = externalTools;
      }

      private readonly Dictionary<int, int> TablePowerOfTwo = new Dictionary<int, int>
      {
         {1, 1},
         {2, 2},
         {4, 3},
         {8, 4},
         {16, 5},
         {32, 6},
         {64, 7},
         {128, 8},
         {256, 9},
         {512, 10},
         {1024, 11},
         {2048, 12},
         {4096, 13},
         {8192, 14},
         {16384, 15},
         {32768, 16},
         {65536, 17}
      };

      internal int FindNearPowerOf2Value(int badValue)
      {
         return TablePowerOfTwo.Keys.Select(e => new KeyValuePair<int, int>(e, Math.Abs(e - badValue))).OrderBy(e => e.Value).First().Key;
      }

      internal void PrepareTreat(List<InformationFile> files, ConfigurationMain mainCfg, List<InformationOrder> orders, List<InformationOrder> discardedOrders, List<InformationOrder> searchResult, bool isBsa)
      {
         var eventName = isBsa ? ProgressAnalyzingArchives : ProgressAnalyzingTextures;
         //           
         Progresser.EventStart(eventName);
         if (mainCfg.IsUseMultithreading)
         {

            var exceptions = new ConcurrentQueue<Exception>();
            Parallel.ForEach(files, (file, state) =>
            {
               try
               {
                  PrepareFileOrder(files, mainCfg, isBsa, file, orders, discardedOrders, searchResult, eventName);
               }
               catch (Exception e)
               {
                  exceptions.Enqueue(e);
                  state.Break();
               }
            });

            if (exceptions.Count > 0)
               throw new AggregateException(exceptions);
         }
         else
         {
            foreach (var file in files)
            {
               PrepareFileOrder(files, mainCfg, isBsa, file, orders, discardedOrders, searchResult, eventName);
            }
         }
         orders.Remove(null);
         if (discardedOrders != null)
            discardedOrders.Remove(null);
         //                                                                                 
         Logger.Log("Files preparation complete. {0} operations queued ", orders.Count);
         Progresser.EventEnd(eventName);
         //        
      }

      private bool Search(List<InformationOrder> result, InformationOrder treated, ConfigurationSearch cfgSearch)
      {
         if (treated.OriginalSize == null)
         {
            Logger.Log("This texture wasn't included in search because there was an error getting its informations: {0}", treated.FileSource.FullName, TypeLog.Warning);
            return false;
         }
         //
         bool isOk = true;
         //              
         //
         if (cfgSearch.IsSearchFormatEnabled)
         {
            if (!string.Equals(treated.OriginalSize.Format, cfgSearch.Format, StringComparison.OrdinalIgnoreCase))
            {
               isOk = false;
            }
         }
         //
         if (cfgSearch.IsSearchNameEnabled)
         {
            if (treated.FileSource.Name.IndexOf(cfgSearch.Name, StringComparison.OrdinalIgnoreCase) < 0)
            {
               isOk = false;
            }
         }
         //
         if (cfgSearch.IsSearchMaxSizeEnabled)
         {
            if (treated.OriginalSize.Width > cfgSearch.MaxSize || treated.OriginalSize.Height > cfgSearch.MaxSize)
            {
               isOk = false;
            }
         }
         //
         if (cfgSearch.IsSearchMinSizeEnabled)
         {
            if (treated.OriginalSize.Width < cfgSearch.MinSize || treated.OriginalSize.Height < cfgSearch.MinSize)
            {
               isOk = false;
            }
         }
         //
         if (cfgSearch.IsSearchMipmapsEnabled)
         {
            if (cfgSearch.IsMipmaps)
            {
               if (treated.OriginalSize.Mipmaps == 1)
               {
                  isOk = false;
               }
            }
            else
            {
               if (treated.OriginalSize.Mipmaps > 1)
               {
                  isOk = false;
               }
            }
         }
         //
         if (cfgSearch.IsSearchPowerOf2)
         {
            if (cfgSearch.IsPowerOf2)
            {
               if (!TablePowerOfTwo.ContainsKey(treated.OriginalSize.Width) || !TablePowerOfTwo.ContainsKey(treated.OriginalSize.Height))
               {
                  isOk = false;
               }
            }
            else
            {
               if (TablePowerOfTwo.ContainsKey(treated.OriginalSize.Width) && TablePowerOfTwo.ContainsKey(treated.OriginalSize.Height))
               {
                  isOk = false;
               }
            }
         }
         //
         if (isOk)
         {
            lock (result)
            {
               result.Add(treated);
            }
         }
         return isOk;
      }

      private void PrepareFileOrder(List<InformationFile> files, ConfigurationMain mainCfg, bool isBsa, InformationFile file,
          List<InformationOrder> orders, List<InformationOrder> discardedOrders, List<InformationOrder> searchResult, string eventName)
      {
         if (mainCfg.IsVerbose)
         {
            Logger.Log(string.Format("Analyzing {0}", file.FileSource.FullName));
         }
         //                  
         Progresser.ChangeProgress(eventName, files.Count);
         //  
         var confirmOrder = false;
         var isBsaMustBeDecompressed = false;
         var order = new InformationOrder(file.FileSource, file.FileTarget) { IsUseBackup = mainCfg.IsUseBackup && mainCfg.IsBackupActivated };
         //  
         if (isBsa)
         {
            if (mainCfg.PassBsa.Enabled && mainCfg.PassBsa.Selection.GetValidation(Path.GetFileNameWithoutExtension(file.FileSource.FullName)))
            {
               order.IsBsaFormatCompressed = ExternalTools.CallBsarchProperty(file.FileSource.FullName, "*COMPRESSED");
               //             
               if (mainCfg.PassBsa.IsCheckFormatIsGameFormat)
               {
                  string searchForFile = "sse";
                  switch (mainCfg.PassBsa.GameParameter)
                  {
                     case "fo4":
                        searchForFile = "Fallout 4 General";
                        break;
                     case "fo4dds":
                        searchForFile = "Fallout 4 DDS";
                        order.IsBsaFormatCompressed = true;
                        break;
                  }
                  isBsaMustBeDecompressed = ExternalTools.CallBsarchProperty(file.FileSource.FullName, searchForFile);
               }
               else if (mainCfg.PassBsa.IsCopyAsLoose)
               {
                  if (mainCfg.PassBsa.IsTreatNonTextureArchives)
                  {
                     isBsaMustBeDecompressed = true;
                  }
                  else
                  {
                     foreach (var texExtensionName in TextureExtensionList.List)
                     {
                        isBsaMustBeDecompressed = ExternalTools.CallBsarchList(file.FileSource.FullName, texExtensionName);
                        if (isBsaMustBeDecompressed)
                           break;
                     }
                  }
               }
               //
               confirmOrder |= isBsaMustBeDecompressed;
            }
         }
         else if (mainCfg.HasTextureOperation() && GetCurrentImageSize(order))
         {
            //                 
            foreach (var scalePass in mainCfg.Passes)
            {
               // Stop here if validation failed
               if (!scalePass.Selection.GetValidation(Path.GetFileNameWithoutExtension(file.FileSource.FullName)))
               {
                  continue;
               }
               //
               if (scalePass.TypePass == TypePass.ApplyGmic)
               {
                  order.IsGmicPass = true;
                  order.GmicCommands.Add(scalePass.Command);
                  confirmOrder = true;
                  continue;
               }
               //
               if (scalePass.TypePass == TypePass.ApplyCustom)
               {
                  order.IsCustomPass = true;
                  order.IsApplyOnPng = scalePass.IsApplyOnPng;
                  order.CustomCommands.Add(scalePass.Command);
                  confirmOrder = true;
                  continue;
               }
               //
               if (scalePass.TypePass == TypePass.Force)
               {
                  order.IsDoTexconv = true;
                  order.TargetSize.TypeTexCompression = scalePass.TypeTexCompression;
                  confirmOrder = true;
                  continue;
               }
               //
               string f = order.TargetSize.Format;
               if (!string.IsNullOrEmpty(scalePass.ForceFormat) && !string.Equals(scalePass.ForceFormat, f, StringComparison.OrdinalIgnoreCase))
               {
                  order.IsDoTexconv = true;
                  order.TargetSize.Format = scalePass.ForceFormat;
                  order.TargetSize.TypeTexCompression = scalePass.TypeTexCompression;
                  confirmOrder = true;
                  continue;
               }
               //

               //   
               int w = order.TargetSize.Width;
               int h = order.TargetSize.Height;
               int m = order.TargetSize.Mipmaps;
               //
               int div = 1;
               int mult = 1;
               bool applyPass = false;
               //
               if (scalePass.TypePass == TypePass.CorrectSize)
               {
                  //correct to nearby power of 2 value  
                  if (!TablePowerOfTwo.ContainsKey(w) || !TablePowerOfTwo.ContainsKey(h))
                  {
                     w = order.TargetSize.Width = FindNearPowerOf2Value(w);
                     h = order.TargetSize.Height = FindNearPowerOf2Value(h);
                     applyPass = true;
                     //
                     if (mainCfg.IsVerbose)
                     {
                        Logger.Log(
                           "{0} size is not a power of 2 and will be resized to width=>{1} height=>{2}",
                           file.FileSource.Name, w, h, TypeLog.Warning);
                     }
                  }
               }
               // find new size and new mipmaps needed       
               if (scalePass.TypePass == TypePass.DownscaleFactor)
               {
                  // division method   
                  if ((w / scalePass.WantedFactor >= scalePass.WantedMinSize && h / scalePass.WantedFactor >= scalePass.WantedMinSize))
                  {
                     div = scalePass.WantedFactor;
                     applyPass = true;
                  }
               }
               //
               if (scalePass.TypePass == TypePass.DownscaleFixed)
               {
                  // fixed size method
                  while ((w / div > scalePass.WantedSize || h / div > scalePass.WantedSize) &&
                         (w / div > scalePass.WantedMinSize && h / div > scalePass.WantedMinSize))
                  {
                     div *= 2;
                     applyPass = true;
                  }
               }
               // find new size and new mipmaps needed       
               if (scalePass.TypePass == TypePass.UpscaleFactor)
               {
                  // division method   
                  if ((w * scalePass.WantedFactor < scalePass.WantedMaxSize && h * scalePass.WantedFactor < scalePass.WantedMaxSize))
                  {
                     mult = scalePass.WantedFactor;
                     applyPass = true;
                  }
               }
               //
               if (scalePass.TypePass == TypePass.UpscaleFixed)
               {
                  // fixed size method
                  while ((w * mult < scalePass.WantedSize || h * mult < scalePass.WantedSize) &&
                         (w * mult < scalePass.WantedMaxSize && h * mult < scalePass.WantedMaxSize))
                  {
                     mult *= 2;
                     applyPass = true;
                  }
               }
               //   
               if (scalePass.TypePass == TypePass.CorrectMipmaps && m > 1)
               {
                  var newSizeToSonsiderFormips = Math.Max(w, h);
                  if (TablePowerOfTwo.ContainsKey(newSizeToSonsiderFormips))
                  {
                     var goodMipmap = TablePowerOfTwo[newSizeToSonsiderFormips];
                     if (m != goodMipmap)
                     {
                        m = goodMipmap;
                        applyPass = true;
                     }
                  }
               }
               //
               if (scalePass.TypePass == TypePass.ForceMipmaps)
               {
                  m = 2;
                  applyPass = true;
               }
               //
               if (applyPass)
               {
                  //           
                  var newWidth = (w / div) * mult;
                  var newHeight = (h / div) * mult;
                  //       
                  if (!TablePowerOfTwo.ContainsKey(newWidth) || !TablePowerOfTwo.ContainsKey(newHeight))
                  {
                     Logger.Log("Texture size is not a power of 2 for {0} (current size: {1}/{2})", file.FileSource.FullName, newWidth, newHeight, TypeLog.Warning);
                     //continue;
                  }
                  //   
                  int newMipmap = 1;
                  // 
                  if (m > 1)
                  {
                     var newSizeToSonsiderFormips = Math.Max(newWidth, newHeight);
                     if (TablePowerOfTwo.ContainsKey(newSizeToSonsiderFormips))
                     {
                        newMipmap = TablePowerOfTwo[newSizeToSonsiderFormips];
                     }
                     else
                     {
                        newMipmap = 0;
                     }
                  }
                  //           
                  order.IsDoTexconv = true;
                  order.TargetSize.Width = newWidth;
                  order.TargetSize.Height = newHeight;
                  order.TargetSize.Mipmaps = newMipmap;
                  confirmOrder = true;
               }
            }
         }
         //
         if (mainCfg.IsRefreshBackup && mainCfg.IsBackupActivated)
         {
            if (((isBsa && isBsaMustBeDecompressed) || !isBsa) && (!order.FileTarget.Exists || order.FileSource.Length > order.FileTarget.Length))
            {
               order.IsRefreshBackup = true;
               confirmOrder = true;
            }
         }
         //
         if (mainCfg.IsRecopyOriginal && mainCfg.IsBackupActivated)
         {
            if (((isBsa && isBsaMustBeDecompressed) || !isBsa) && (!order.FileSource.Exists || order.FileTarget.Length > order.FileSource.Length))
            {
               order.IsRecopyOriginal = true;
               confirmOrder = true;
            }
         }
         //
         if (order != null)
         {
            bool searchValidation = true;
            //
            if (mainCfg.Search.IsSearchEnabled && !isBsa)
            {
               searchValidation = Search(searchResult, order, mainCfg.Search);
            }
            //
            if (confirmOrder && (searchValidation || !mainCfg.Search.IsApplySearchToProcess))
            {
               lock (orders)
               {
                  orders.Add(order);
               }
            }
            else
            {
               if (discardedOrders != null)
               {
                  lock (discardedOrders)
                  {
                     discardedOrders.Add(order);
                  }
               }
            }
         }
      }



      //private bool GetCurrentImageSizeTimed(InformationOrder order)
      //{
      //   int w;
      //   int h;
      //   int m;
      //   string f;
      //   //
      //   var textureSource = order.IsUseBackup && order.FileTarget != null ? order.FileTarget.FullName : order.FileSource.FullName;
      //   var t1 = Stopwatch.StartNew();
      //   var tDxDiag = ExternalTools.CallDxDiag(textureSource);
      //   t1.Stop();
      //   // if dxDiag return a fail value, adds to the fails list
      //   // it seams the dds texture is not really a dds texture format    
      //   var t2 = Stopwatch.StartNew();
      //   var wIndex = tDxDiag.IndexOf("width = ", StringComparison.OrdinalIgnoreCase);
      //   var hIndex = tDxDiag.IndexOf("height = ", StringComparison.OrdinalIgnoreCase);
      //   var mIndex = tDxDiag.IndexOf("mipLevels = ", StringComparison.OrdinalIgnoreCase);
      //   var fIndex = tDxDiag.IndexOf("format = ", StringComparison.OrdinalIgnoreCase);
      //   // var lines = tDxDiag.Split('\n');
      //   //
      //   //Alternative: if (tDxDiag.Contains("FAILED"))
      //   if (wIndex < 0 || hIndex < 0 || mIndex < 0 || fIndex < 0)
      //   {
      //      Logger.Log("Invalid DxDiag data: {0}\n Returned value: {1}", textureSource, tDxDiag, TypeLog.Error);
      //      return false;
      //   }
      //   // find current width, height, mimaps
      //   bool parsingNoError = true;
      //   //
      //   parsingNoError &= int.TryParse(tDxDiag.Substring(wIndex + 8, 4), out w);
      //   parsingNoError &= int.TryParse(tDxDiag.Substring(hIndex + 9, 4), out h);
      //   parsingNoError &= int.TryParse(tDxDiag.Substring(mIndex + 12, 2), out m);

      //   var textFromFormat = tDxDiag.Substring(fIndex + 9);
      //   var lineIndex = Math.Min(textFromFormat.IndexOf("\n", StringComparison.OrdinalIgnoreCase), textFromFormat.IndexOf("\r", StringComparison.OrdinalIgnoreCase));
      //   f = textFromFormat.Substring(0, lineIndex);
      //   t2.Stop();
      //   //
      //   if (!parsingNoError)
      //   {
      //      Logger.Log("Invalid Formating of DxDiag data: {0}\n Returned value: {1}", textureSource, tDxDiag, TypeLog.Error);
      //      return false;
      //   }
      //   Logger.Log("Ticks dxdiag {0} parse {1}", t1.ElapsedTicks, t2.ElapsedTicks, TypeLog.Normal);
      //   //
      //   order.TargetSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f };
      //   order.OriginalSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f, TypeTexCompression = TypeTexCompression.None };
      //   return true;
      //}

      //private bool GetCurrentImageSize2(InformationOrder order)
      //{
      //   int w;
      //   int h;
      //   int m;
      //   string f;
      //   //
      //   var textureSource = order.IsUseBackup && order.FileTarget != null ? order.FileTarget.FullName : order.FileSource.FullName;
      //   var tDxDiag = ExternalTools.CallDxDiag(textureSource);
      //   // if dxDiag return a fail value, adds to the fails list
      //   // it seams the dds texture is not really a dds texture format  
      //   var wIndex = tDxDiag.IndexOf("width = ", StringComparison.OrdinalIgnoreCase);
      //   var hIndex = tDxDiag.IndexOf("height = ", StringComparison.OrdinalIgnoreCase);
      //   var mIndex = tDxDiag.IndexOf("mipLevels = ", StringComparison.OrdinalIgnoreCase);
      //   var fIndex = tDxDiag.IndexOf("format = ", StringComparison.OrdinalIgnoreCase);
      //   // var lines = tDxDiag.Split('\n');
      //   //
      //   //Alternative: if (tDxDiag.Contains("FAILED"))
      //   if (wIndex < 0 || hIndex < 0 || mIndex < 0 || fIndex < 0)
      //   {
      //      Logger.Log("Invalid DxDiag data: {0}\n Returned value: {1}", textureSource, tDxDiag, TypeLog.Error);
      //      return false;
      //   }
      //   // find current width, height, mimaps
      //   bool parsingNoError = true;
      //   //
      //   parsingNoError &= int.TryParse(tDxDiag.Substring(wIndex + 8, 4), out w);
      //   parsingNoError &= int.TryParse(tDxDiag.Substring(hIndex + 9, 4), out h);
      //   parsingNoError &= int.TryParse(tDxDiag.Substring(mIndex + 12, 2), out m);

      //   var textFromFormat = tDxDiag.Substring(fIndex + 9);
      //   var lineIndex = Math.Min(textFromFormat.IndexOf("\n", StringComparison.OrdinalIgnoreCase), textFromFormat.IndexOf("\r", StringComparison.OrdinalIgnoreCase));
      //   f = textFromFormat.Substring(0, lineIndex);
      //   //
      //   if (!parsingNoError)
      //   {
      //      Logger.Log("Invalid Formating of DxDiag data: {0}\n Returned value: {1}", textureSource, tDxDiag, TypeLog.Error);
      //      return false;
      //   }
      //   //
      //   order.TargetSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f };
      //   order.OriginalSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f, TypeTexCompression = TypeTexCompression.None };
      //   return true;
      //}

      private bool GetCurrentImageSize(InformationOrder order)
      {
         var textureSource = order.IsUseBackup && order.FileTarget != null ? order.FileTarget.FullName : order.FileSource.FullName;
         int exitCode;
         //
         var tDxDiag = ExternalTools.CallDxDiag(textureSource, out exitCode);
         if (exitCode != 0)
         {
            Logger.Log("DxDiag failed for {0}\nExit code: {1}\nDxDiag Output:\n{2}", textureSource, exitCode, tDxDiag, TypeLog.Error);
            return false;
         }
         //
         var lines = tDxDiag.Split('\n');
         bool parsingNoError = true;
         if (lines.Length < 11 || lines[4].Length < 17 || lines[5].Length < 17 || lines[7].Length < 17 || lines[10].Length < 17)
         {
            Logger.Log("Invalid DxDiag data for {0}\nDxDiag Output:\n{1}", textureSource, tDxDiag, TypeLog.Error);
            return false;
         }
         //
         int w, h, m;
         parsingNoError &= int.TryParse(lines[4].Substring(16, lines[4].Length - 17), out w);
         parsingNoError &= int.TryParse(lines[5].Substring(16, lines[5].Length - 17), out h);
         parsingNoError &= int.TryParse(lines[7].Substring(16, lines[7].Length - 17), out m);
         var f = lines[10].Substring(16, lines[10].Length - 17);
         //
         if (!parsingNoError)
         {
            Logger.Log("Invalid DxDiag data format for {0}\nDxDiag Output:\n{1}", textureSource, tDxDiag, TypeLog.Error);
            return false;
         }
         //
         order.TargetSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f };
         order.OriginalSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f, TypeTexCompression = TypeTexCompression.None };
         return true;
      }

      //private bool GetCurrentImageSize3(InformationOrder order)
      //{
      //   int w, h, m;
      //   string f;
      //   //
      //   var textureSource = order.IsUseBackup && order.FileTarget != null ? order.FileTarget.FullName : order.FileSource.FullName;
      //     //
      //   if (ExternalTools.CallDxDiagDirect(textureSource, out w, out h, out m, out f))
      //   {
      //      Logger.Log("DxDiag unable to get info: {0}", textureSource, TypeLog.Error);
      //      return false;
      //   }
      //   //
      //   order.TargetSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f };
      //   order.OriginalSize = new InformationImage { Width = w, Height = h, Mipmaps = m, Format = f, TypeTexCompression = TypeTexCompression.None };
      //   return true;
      //}

      public void PrepareUnmerge(ConfigurationMain mainCfg, List<InformationFileDeletion> deletes)
      {
         if (mainCfg.IsVerbose)
         {
            Logger.Log("Preparing a mods unmerge");
         }
         //
         var source = new DirectoryInfo(mainCfg.PathSource);
         //    
         var target = new DirectoryInfo(mainCfg.PathMergeDirectory);
         //       
         List<DirectoryInfo> validatedDirectories = new List<DirectoryInfo>();
         foreach (DirectoryInfo directoryInfo in source.GetDirectories())
         {
            if (mainCfg.Selection.GetValidation(directoryInfo.Name))
            {
               validatedDirectories.Add(directoryInfo);
            }
         }
         //
         foreach (DirectoryInfo directoryInfo in validatedDirectories)
         {
            FileUtils.DeleteIfFound(directoryInfo, target, deletes);
         }
      }

      public void PrepareMerge(ConfigurationMain mainCfg, List<InformationCopy> copies)
      {
         if (mainCfg.IsVerbose)
         {
            Logger.Log("Preparing a mods merge");
         }
         //
         var source = new DirectoryInfo(mainCfg.PathSource);
         //    
         var target = new DirectoryInfo(mainCfg.PathMergeDirectory);
         if (!target.Exists)
            target.Create();
         //
         List<DirectoryInfo> validatedDirectories = new List<DirectoryInfo>();
         foreach (DirectoryInfo directoryInfo in source.GetDirectories())
         {
            if (mainCfg.Selection.GetValidation(directoryInfo.Name))
            {
               validatedDirectories.Add(directoryInfo);
            }
         }
         //              
         List<InformationMerge> merges = new List<InformationMerge>();
         //
         if (string.IsNullOrEmpty(mainCfg.PathMergePriorityFile))
         {
            foreach (DirectoryInfo directoryInfo in validatedDirectories)
            {
               merges.Add(new InformationMerge(directoryInfo, target));
            }
         }
         else
         {
            //read cvs file        
            List<InformationPriority> priorities = new List<InformationPriority>();
            using (var reader = new StreamReader(mainCfg.PathMergePriorityFile))
            {
               while (!reader.EndOfStream)
               {
                  var line = reader.ReadLine();
                  var values = line.Split(',');

                  int prio;
                  if (!int.TryParse(values[0].Replace('"', ' ').Trim(), out prio))
                  {
                     continue;
                  }
                  priorities.Add(new InformationPriority(prio, values[1].Replace('"', ' ').Trim()));
               }
            }
            //           
            //foreach (var priority in priorities.OrderBy(e => e.Priority))
            //{
            //   var dir = validatedDirectories.FirstOrDefault(d => Equals(d.Name, priority.ModName));
            //   if (dir != null)
            //   {
            //      merges.Add(new InformationMerge(dir, target));
            //      if (mainCfg.IsVerbose)
            //      {
            //         Logger.Log(string.Format("Mod \"{0}\" added with a priority of {1}", priority.ModName, priority.Priority));
            //      }
            //   }
            //   else
            //   {
            //      throw new KeyNotFoundException(string.Format("Mod {0} wasn't found in the source directory", priority.ModName));
            //   }
            //}
            //         
            List<InformationPriority> confirmedPriorities = new List<InformationPriority>();
            foreach (var vdir in validatedDirectories)
            {
               var prio = priorities.FirstOrDefault(d => Equals(d.ModName, vdir.Name));
               if (prio != null)
               {
                  confirmedPriorities.Add(prio);
               }
               else
               {
                  throw new KeyNotFoundException(string.Format("Mod {0} wasn't found in the priority file", vdir.Name));
               }
            }
            foreach (InformationPriority priority in confirmedPriorities.OrderBy(e => e.Priority))
            {
               var dir = validatedDirectories.FirstOrDefault(d => Equals(d.Name, priority.ModName));
               if (dir != null)
               {
                  merges.Add(new InformationMerge(dir, target));

               }
               else
               {
                  throw new DirectoryNotFoundException(string.Format("Directory for {0} wasn't found", priority.ModName));
               }
               //
               if (mainCfg.IsVerbose)
               {
                  Logger.Log(string.Format("Mod \"{0}\" added with a priority of {1}", priority.ModName, priority.Priority));
               }
            }
         }
         //       
         Dictionary<long, InformationCopy> preparedCopies = new Dictionary<long, InformationCopy>();
         if (mainCfg.IsMergeDeleteIfNotInSource)
         {
            Progresser.EventStart(ProgressMergeDeleteIfNotInSource);
            FileUtils.AnalyseSourceForDeleteWhenMerge(target, preparedCopies, mainCfg.IsVerbose);
            Progresser.EventEnd(ProgressMergeDeleteIfNotInSource);
         }
         //      
         Progresser.EventStart(ProgressListingMergingOp);
         if (mainCfg.IsVerbose)
         {
            Logger.Log("Listing merging copies operations");
         }
         //
         foreach (var merge in merges)
         {
            //
            if (mainCfg.IsVerbose)
            {
               Logger.Log("Listing copies operations for {0}", merge.DirSource.Name);
            }
            //            
            Progresser.ChangeProgress(ProgressListingMergingOp, merges.Count);
            FileUtils.CopyDirContent(merge.DirSource, merge.DirTarget, preparedCopies, mainCfg.IsMergeAssertCase);
            //                              
         }
         //
         copies.AddRange(preparedCopies.Values.Where(e => e.Confirmed));
         //                            
         if (mainCfg.IsVerbose)
         {
            Logger.Log(string.Format("Copying mods files into merged directory: {0} copy operations", copies.Count));
         }
         //
         Progresser.EventEnd(ProgressListingMergingOp);
      }
   }
}