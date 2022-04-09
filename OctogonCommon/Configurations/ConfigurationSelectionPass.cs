using System;
using OctagonCommon.Statics;

namespace OctagonCommon.Configurations
{

   public class ConfigurationSelectionPass
   {
      public TypeSelection TypeSelection { get; set; }

      public string Pattern { get; set; }
      public int Occurence { get; set; }
      public bool IsCheckFullPath { get; set; }

      public ConfigurationSelectionPass(TypeSelection typeSelection, string pattern, int occurence, bool isCheckFullPath)
      {
         TypeSelection = typeSelection;
         Pattern = pattern;
         Occurence = occurence;
         IsCheckFullPath = isCheckFullPath;
      }         
                       
      public bool IsStartValidated()
      {
         return TypeSelection != TypeSelection.IncludeOccurenceLess && TypeSelection != TypeSelection.IncludeOccurenceEqualOrMore && TypeSelection != TypeSelection.IncludeExact && TypeSelection != TypeSelection.Include && TypeSelection != TypeSelection.IncludeEnd && TypeSelection != TypeSelection.IncludeStart;
      }

      public bool IsOccurenceEnabled()
      {
         return TypeSelection == TypeSelection.IncludeOccurenceLess || TypeSelection == TypeSelection.IncludeOccurenceEqualOrMore|| TypeSelection== TypeSelection.ExcludeOccurenceEqualOrMore|| TypeSelection == TypeSelection.ExcludeOccurenceLess;
      }
           
      public bool Validate(bool state, string name, string fullPath)
      {
         string validate = IsCheckFullPath ? fullPath : name;

         switch (TypeSelection)
         {
            case TypeSelection.IncludeExact:
               if (validate.Equals(Pattern, StringComparison.OrdinalIgnoreCase))
               {
                  return true;
               }
               break;
            case TypeSelection.ExcludeExact:
               if (validate.Equals(Pattern, StringComparison.OrdinalIgnoreCase))
               {
                  return false;
               }
               break;
            case TypeSelection.Include:
               if (validate.IndexOf(Pattern, StringComparison.OrdinalIgnoreCase) >= 0)
               {
                  return true;
               }
               break;
            case TypeSelection.IncludeStart:
               if (validate.StartsWith(Pattern, StringComparison.OrdinalIgnoreCase))
               {
                  return true;
               }
               break;
            case TypeSelection.IncludeEnd:
               if (validate.EndsWith(Pattern, StringComparison.OrdinalIgnoreCase))
               {
                  return true;
               }
               break;
            case TypeSelection.Exclude:
               if (validate.IndexOf(Pattern, StringComparison.OrdinalIgnoreCase) >= 0)
               {
                  return false;
               }
               break;
            case TypeSelection.ExcludeStart:
               if (validate.StartsWith(Pattern, StringComparison.OrdinalIgnoreCase))
               {
                  return false;
               }
               break;
            case TypeSelection.ExcludeEnd:
               if (validate.EndsWith(Pattern, StringComparison.OrdinalIgnoreCase))
               {
                  return false;
               }
               break;
            case TypeSelection.IncludeOccurenceLess:
               if (CountStr(validate, Pattern) < Occurence)
               {
                  return true;
               }
               break;
            case TypeSelection.IncludeOccurenceEqualOrMore:
               if (CountStr(validate, Pattern) >= Occurence)
               {
                  return true;
               }
               break;
            case TypeSelection.ExcludeOccurenceLess:
               if (CountStr(validate, Pattern) < Occurence)
               {
                  return false;
               }
               break;
            case TypeSelection.ExcludeOccurenceEqualOrMore:
               if (CountStr(validate, Pattern) >= Occurence)
               {
                  return false;
               }
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
         return state;
      }
               
      private int CountStr(string source, string search)
      {
         return (source.Length - source.Replace(search, "").Length) / search.Length;
      }
   }



}