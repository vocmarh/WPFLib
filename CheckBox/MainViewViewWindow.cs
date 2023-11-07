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
        private string _searchText;
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
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; NotifyPropertyChanged(nameof(SearchText)); }
        }

        private bool filterSearchText(object item)
        {
            CategoryModel catModel = (CategoryModel)item;
            if (SearchText != null || SearchText != "")
            {
                return catModel.CategoryName.ToUpper().Contains(SearchText.ToUpper());

            }
            else { return true; }
        }

        public DelegateCommand ExportExcel { get; }
        public DelegateCommand SearchCategoryCommand { get; }

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
            SearchCategoryCommand = new DelegateCommand(OnSearchCategoryCommand);

            ListOfCategories listOfCategories = new ListOfCategories();
            listOfCategories.GetCategories(_commandData, Categories);            
        }

        private void OnSearchCategoryCommand()
        {
            // Получите введенный текст из SearchText
            //string searchText = SearchText;

            // Выполните поиск категории по имени в коллекции Categories
            CategoryModel foundCategory = Categories.FirstOrDefault(category => category.CategoryName.Contains(_searchText));

            // Далее, вы можете обновить ваш интерфейс или выполнить другие действия в зависимости от найденной категории
            if (foundCategory != null)
            {
                // Категория найдена
                // Дополнительные действия, например, установка флага IsSelected для категории и обновление интерфейса
                foundCategory.IsSelected = true; // Пример установки флага IsSelected
            }            
        }

        private void OnExportExcel()
        {
            ExcelExporters exporters = new ExcelExporters();
            exporters.GetElementToExcel(Data);
        }
        private void SearchCategory()
        {
            // Получите введенный текст из SearchText
            string searchText = SearchText;

            // Выполните поиск категории по имени в коллекции Categories
            CategoryModel foundCategory = Categories.FirstOrDefault(category => category.CategoryName.Contains(searchText));

            // Далее, вы можете обновить ваш интерфейс или выполнить другие действия в зависимости от найденной категории
            if (foundCategory != null)
            {
                // Категория найдена
                // Дополнительные действия, например, установка флага IsSelected для категории и обновление интерфейса
                foundCategory.IsSelected = true; // Пример установки флага IsSelected
            }
            else
            {
                // Категория не найдена
                // Дополнительные действия, если необходимо
            }
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
                        List<Element> elements = getElemByCat.GetElementsByCategoryName(category.CategoryName, _commandData);
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
