using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Octagon.Tools;
using OctagonCommon.Configurations;
using OctagonCommon.Statics;

namespace Octagon.Models
{
   public class ModelConfigurationSelection : AModel
   {

      public ConfigurationSelection Selection { get; private set; }

      public ModelConfigurationSelection(ConfigurationSelection selection)
      {
         VisibilityShowInfo = Visibility.Collapsed;
         Selection = selection;
         Filters = new ObservableCollection<ModelConfigurationSelectionPass>(selection.Filters.Select(e => new ModelConfigurationSelectionPass(e)));
         DeleteItemCommand = new DelegateCommand(DeleteFilter);
      }

      public DelegateCommand DeleteItemCommand { get; set; }

      public ObservableCollection<ModelConfigurationSelectionPass> Filters { get; private set; }

      public void ChangeShowInfo()
      {
         VisibilityShowInfo = VisibilityShowInfo == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
      }

      private Visibility _visibilityShowInfo;
      public Visibility VisibilityShowInfo
      {
         get { return _visibilityShowInfo; }
         set
         {
            if (_visibilityShowInfo != value)
            {
               _visibilityShowInfo = value;
               OnPropertyChanged("VisibilityShowInfo");
            }
         }
      }

      public void AddFilter()
      {
         var newElement = Selection.AddFilter(TypeSelection.Include, string.Empty, 1,false);
         Filters.Add(new ModelConfigurationSelectionPass(newElement));
      }

      public void Invert()
      {
         foreach (ModelConfigurationSelectionPass modelConfigurationSelectionPass in Filters)
         {
            modelConfigurationSelectionPass.Invert();
         }
      }

      private void DeleteFilter(object obj)
      {
         var filterSelected = obj as ModelConfigurationSelectionPass;
         if (filterSelected != null)
         {
            Selection.RemoveFilter(filterSelected.SelectionPass);
            Filters.Remove(filterSelected);
         }
      }

      public void Save(string file)
      {
         Selection.Write(file);
      }

      public void Open(string file)
      {
         Selection.Read(file);
         Filters.Clear();
         foreach (ConfigurationSelectionPass configurationSelectionPass in Selection.Filters)
         {
            Filters.Add(new ModelConfigurationSelectionPass(configurationSelectionPass));
         }
      }
   }
}
