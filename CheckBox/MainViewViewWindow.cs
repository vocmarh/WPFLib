using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CheckBox.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;


namespace CheckBox
{
    public class MainViewViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ElementData> data;
        private ObservableCollection<CategoryModel> categories;
        private bool isSelected = true;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; NotifyPropertyChanged(nameof(IsSelected)); }
        }           
        public ObservableCollection<ElementData> Data 
        {
            get => data;
            set { data = value; NotifyPropertyChanged(nameof(Data)); }
        }
        public ObservableCollection<CategoryModel> Categories
        {
            get => categories;
            set { categories = value; NotifyPropertyChanged(nameof(Categories));}
        }

        public DelegateCommand ExportExcel { get; }

        private void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }            

        private ExternalCommandData _commandData;       
        public DelegateCommand GetDataBase { get; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            GetDataBase = new DelegateCommand(OnGetDataBase);
            Categories = new ObservableCollection<CategoryModel>();
            ExportExcel = new DelegateCommand(OnExportExcel);

            ListOfCategories listOfCategories = new ListOfCategories();
            listOfCategories.GetCategories(_commandData, Categories);            
        }

        private void OnExportExcel()
        {
            ExcelExporters exporters = new ExcelExporters();
            exporters.GetElementToExcel(Data);
        }

        private void OnGetDataBase()
        {
            Data = new ObservableCollection<ElementData>();
            ProcessElements processElements = new ProcessElements();

            GetElemByCat getElemByCat = new GetElemByCat();
            
            if (isSelected)
            {
                foreach (var category in Categories)
                {
                    if (category.IsSelected)
                    {
                        List<Element> elements = getElemByCat.GetElementsByCategoryName(category.Name, _commandData);
                        if (elements != null)
                        {
                            processElements.GetProcessElements(elements, Data, _commandData);
                        }
                    }
                }                
            }
        }    

        public event EventHandler HideRequest;

        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;

        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CloseRequest;
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }

}
