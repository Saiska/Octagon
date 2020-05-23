using System.Collections.Generic;
using System.ComponentModel;
using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class AModel : INotifyPropertyChanged
   {
      public static List<ProxyTypeSelection> ProxyTypeSelections { get; private set; }
      public static List<ProxyBsaGame> ProxyBsaGames { get; private set; }
      public static List<ProxyTypeTexCompression> ProxyTypeTexCompressions { get; private set; }
      public static List<string> ProxyDxgiFormat { get; private set; }
      public static List<ProxyInteger> ProxyDownscaleSize { get; private set; }
      public static List<ProxyInteger> ProxyDownscaleFactor { get; private set; }    
      public static List<ProxyInteger> ProxyUpscaleSize { get; private set; }
      public static List<ProxyInteger> ProxyUpscaleFactor { get; private set; }      
      public static List<ProxyInteger> ProxyMinSize { get; private set; }
      public static List<ProxyInteger> ProxyMaxSize { get; private set; }
      public static List<ProxyInteger> ProxySize { get; private set; }

      static AModel()
      {
         ProxyTypeTexCompressions = new List<ProxyTypeTexCompression>()
         {
            new ProxyTypeTexCompression(TypeTexCompression.None,"Don't change compression") ,    
            new ProxyTypeTexCompression(TypeTexCompression.BC13Uni,"Uses uniform weighting rather than perceptual (BC1-BC3)") ,
            new ProxyTypeTexCompression(TypeTexCompression.BC13Dith,"Uses dithering (BC1-BC3)") ,
            new ProxyTypeTexCompression(TypeTexCompression.BC7Min,"Uses minimal compression (BC7: uses just mode 6)") ,
            new ProxyTypeTexCompression(TypeTexCompression.BC7Max,"Uses maximum compression (BC7: enables mode 0 & 2 usage)") ,      
         };
         //
         ProxyTypeSelections = new List<ProxyTypeSelection>()
         {
            new ProxyTypeSelection(TypeSelection.Include,"Include if contains") ,    
            new ProxyTypeSelection(TypeSelection.IncludeStart,"Include if start by") ,
            new ProxyTypeSelection(TypeSelection.IncludeEnd,"Include if end by") ,      
            new ProxyTypeSelection(TypeSelection.IncludeExact,"Include if is exactly"),  
            new ProxyTypeSelection(TypeSelection.IncludeOccurenceLess,"Include if less occurences"),  
            new ProxyTypeSelection(TypeSelection.IncludeOccurenceEqualOrMore,"Include if equal or more occurences"),  
            new ProxyTypeSelection(TypeSelection.Exclude,"Exclude if contains") ,
            new ProxyTypeSelection(TypeSelection.ExcludeStart,"Exclude if start by") ,
            new ProxyTypeSelection(TypeSelection.ExcludeEnd,"Exclude if end by"),  
            new ProxyTypeSelection(TypeSelection.ExcludeExact,"Exclude if is exactly"),  
            new ProxyTypeSelection(TypeSelection.ExcludeOccurenceLess,"Exclude if less occurences"),  
            new ProxyTypeSelection(TypeSelection.ExcludeOccurenceEqualOrMore,"Exclude if equal or more occurences"),     
         };
         //
         ProxyBsaGames = new List<ProxyBsaGame>()
         {
            new ProxyBsaGame( "tes3","Morrowind archive format"),
            new ProxyBsaGame( "tes4","Oblivion archive format "),
            new ProxyBsaGame( "fo3","Fallout 3 archive format "),
            new ProxyBsaGame( "fnv","Fallout: New Vegas archive format "),
            new ProxyBsaGame( "tes5","Skyrim LE archive format (fo3/fnv/tes5 are technically the same)"),
            new ProxyBsaGame( "sse","Skyrim Special Edition archive format "),
            new ProxyBsaGame( "fo4","Fallout 4 General archive format "),
            new ProxyBsaGame( "fo4dds","Fallout 4 DDS archive format (streamed DDS textures mipmaps) ") 
         };
         //
         ProxyDxgiFormat = DxgiFormatList.Formats;
         //
         ProxyDownscaleFactor = new List<ProxyInteger>()
         {
            new ProxyInteger(2,"Divide per 2"),
            new ProxyInteger(4,"Divide per 4"),
            new ProxyInteger(8,"Divide per 8"),
            new ProxyInteger(16,"Divide per 16"),
            new ProxyInteger(32,"Divide per 32"),
            new ProxyInteger(64,"Divide per 64"),
            new ProxyInteger(128,"Divide per 128"),
         };
         //
         ProxyDownscaleSize = new List<ProxyInteger>()
         {                                          
            new ProxyInteger(32,"Max 32 Pixels"),
            new ProxyInteger(64,"Max 64 Pixels"),
            new ProxyInteger(128,"Max 128 Pixels"),
            new ProxyInteger(256,"Max 256 Pixels"),
            new ProxyInteger(512,"Max 512 Pixels"),
            new ProxyInteger(1024,"Max 1024 Pixels"),
            new ProxyInteger(2048,"Max 2048 Pixels"),
            new ProxyInteger(4096,"Max 4096 Pixels"),
            new ProxyInteger(8192,"Max 8192 Pixels"),
            new ProxyInteger(16384,"Max 16384 Pixels"),
         };
         //
         ProxyUpscaleFactor = new List<ProxyInteger>()
         {
            new ProxyInteger(2,"Multiply per 2"),
            new ProxyInteger(4,"Multiply per 4"),
            new ProxyInteger(8,"Multiply per 8"),
            new ProxyInteger(16,"Multiply per 16"),
            new ProxyInteger(32,"Multiply per 32"),
            new ProxyInteger(64,"Multiply per 64"),
            new ProxyInteger(128,"Multiply per 128"),
         };
         //
         ProxyUpscaleSize= new List<ProxyInteger>()
         {                                          
            new ProxyInteger(32,"Min 32 Pixels"),
            new ProxyInteger(64,"Min 64 Pixels"),
            new ProxyInteger(128,"Min 128 Pixels"),
            new ProxyInteger(256,"Min 256 Pixels"),
            new ProxyInteger(512,"Min 512 Pixels"),
            new ProxyInteger(1024,"Min 1024 Pixels"),
            new ProxyInteger(2048,"Min 2048 Pixels"),
            new ProxyInteger(4096,"Min 4096 Pixels"),
            new ProxyInteger(8192,"Min 8192 Pixels"),
            new ProxyInteger(16384,"Min 16384 Pixels"),
         };
         //
         ProxyMinSize = new List<ProxyInteger>()
         {                                                    
            new ProxyInteger(32,"Minimum size: 32 Pixels"),
            new ProxyInteger(64,"Minimum size: 64 Pixels"),
            new ProxyInteger(128,"Minimum size: 128 Pixels"),
            new ProxyInteger(256,"Minimum size: 256 Pixels"),
            new ProxyInteger(512,"Minimum size: 512 Pixels"),
            new ProxyInteger(1024,"Minimum size: 1024 Pixels"),
            new ProxyInteger(2048,"Minimum size: 2048 Pixels"),
            new ProxyInteger(4096,"Minimum size: 4096 Pixels"),
            new ProxyInteger(8192,"Minimum size: 8192 Pixels"),
            new ProxyInteger(16384,"Minimum size: 16384 Pixels"),
         };
         //
         ProxySize = new List<ProxyInteger>()
         {                                                    
            new ProxyInteger(2,"2 Pixels"),
            new ProxyInteger(4,"4 Pixels"),
            new ProxyInteger(8,"8 Pixels"),
            new ProxyInteger(16,"16 Pixels"),
            new ProxyInteger(32,"32 Pixels"),
            new ProxyInteger(64,"64 Pixels"),
            new ProxyInteger(128,"128 Pixels"),
            new ProxyInteger(256,"256 Pixels"),
            new ProxyInteger(512,"512 Pixels"),
            new ProxyInteger(1024,"1024 Pixels"),
            new ProxyInteger(2048,"2048 Pixels"),
            new ProxyInteger(4096,"4096 Pixels"),
            new ProxyInteger(8192,"8192 Pixels"),
            new ProxyInteger(16384,"16384 Pixels"),
         };
         //
         ProxyMaxSize = new List<ProxyInteger>()
         {                                    
            new ProxyInteger(256,"Maximum size: 256 Pixels"),                      
            new ProxyInteger(512,"Maximum size: 512 Pixels"),
            new ProxyInteger(1024,"Maximum size: 1024 Pixels"),
            new ProxyInteger(2048,"Maximum size: 2048 Pixels"),
            new ProxyInteger(4096,"Maximum size: 4096 Pixels"),
            new ProxyInteger(8192,"Maximum size: 8192 Pixels"),
            new ProxyInteger(16384,"Maximum size: 16384 Pixels"),
         };
      }

      public event PropertyChangedEventHandler PropertyChanged;

      protected virtual void OnPropertyChanged(string propertyName)
      {
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
      }
   }
}
