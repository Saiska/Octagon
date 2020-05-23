using System;
using System.Linq;
using System.Windows;
using OctagonCommon.Configurations;
using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class ModelConfigurationSelectionPass : AModel
   {
      public ConfigurationSelectionPass SelectionPass { get; private set; }

      public ModelConfigurationSelectionPass(ConfigurationSelectionPass selectionPass)
      {
         SelectionPass = selectionPass;
      }

      public string Title
      {
         get
         {
            var proxy = ProxyTypeSelections.Single(e => e.TypeSelection == TypeSelection);
            var text = proxy == null ? "Error no valid selector found ?" : proxy.Text;
            return string.Format("{0}: \"{1}\"", text, Pattern);
         }
      }

      public Visibility VisibilityOccurence
      {
         get
         {
            return SelectionPass.IsOccurenceEnabled() ? Visibility.Visible : Visibility.Collapsed;
         }
      }

      public TypeSelection TypeSelection
      {
         get { return SelectionPass.TypeSelection; }
         set
         {
            if (SelectionPass.TypeSelection != value)
            {
               SelectionPass.TypeSelection = value;
               OnPropertyChanged("TypeSelection");
               OnPropertyChanged("Title");
               OnPropertyChanged("VisibilityOccurence");
               OnPropertyChanged("FilterType");
            }
         }
      }

      public string Pattern
      {
         get { return SelectionPass.Pattern; }
         set
         {
            if (SelectionPass.Pattern != value)
            {
               SelectionPass.Pattern = value;
               OnPropertyChanged("Pattern");
               OnPropertyChanged("Title");
               OnPropertyChanged("FilterType");
            }
         }
      }

      public int Occurence
      {
         get { return SelectionPass.Occurence; }
         set
         {
            if (SelectionPass.Occurence != value)
            {
               SelectionPass.Occurence = value;
               OnPropertyChanged("Occurence");
            }
         }
      }

      public int FilterType
      {
         get
         {
            switch (TypeSelection)
            {
               case TypeSelection.Include:
                  return 0;
               case TypeSelection.IncludeStart:
                  return 1;
               case TypeSelection.IncludeEnd:
                  return 2;
               case TypeSelection.Exclude:
                  return 3;
               case TypeSelection.ExcludeStart:
                  return 4;
               case TypeSelection.ExcludeEnd:
                  return 5;
               case TypeSelection.IncludeExact:
                  return 6;
               case TypeSelection.ExcludeExact:
                  return 7;
               case TypeSelection.IncludeOccurenceLess:
                  return 8;
               case TypeSelection.ExcludeOccurenceLess:
                  return 9;
               case TypeSelection.IncludeOccurenceEqualOrMore:
                  return 10;
               case TypeSelection.ExcludeOccurenceEqualOrMore:
                  return 11;
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }
      }

      public void Invert()
      {
         switch (TypeSelection)
         {
            case TypeSelection.Include:
               TypeSelection = TypeSelection.Exclude;
               break;
            case TypeSelection.IncludeStart:
               TypeSelection = TypeSelection.ExcludeStart;
               break;
            case TypeSelection.IncludeEnd:
               TypeSelection = TypeSelection.ExcludeEnd;
               break;
            case TypeSelection.Exclude:
               TypeSelection = TypeSelection.Include;
               break;
            case TypeSelection.ExcludeStart:
               TypeSelection = TypeSelection.IncludeStart;
               break;
            case TypeSelection.ExcludeEnd:
               TypeSelection = TypeSelection.IncludeEnd;
               break;
            case TypeSelection.IncludeExact:
               TypeSelection = TypeSelection.ExcludeExact;
               break;
            case TypeSelection.ExcludeExact:
               TypeSelection = TypeSelection.IncludeExact;
               break;
            case TypeSelection.IncludeOccurenceLess:
               TypeSelection = TypeSelection.ExcludeOccurenceLess;
               break;
            case TypeSelection.ExcludeOccurenceLess:
               TypeSelection = TypeSelection.IncludeOccurenceLess;
               break;
            case TypeSelection.IncludeOccurenceEqualOrMore:
               TypeSelection = TypeSelection.ExcludeOccurenceEqualOrMore;
               break;
            case TypeSelection.ExcludeOccurenceEqualOrMore:
               TypeSelection = TypeSelection.IncludeOccurenceEqualOrMore;
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }
}