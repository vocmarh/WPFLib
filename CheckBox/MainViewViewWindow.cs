using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CheckBox
{
    public class MainViewViewModel : BindableBase
    {
        private bool _isCategoryWall;
        private bool _isCategoryFloor;

        public bool IsCategoryWall
        {
            get { return _isCategoryWall; }
            set { SetProperty(ref _isCategoryWall, value); }
        }

        public bool IsCategoryFloor
        {
            get { return _isCategoryFloor; }
            set { SetProperty(ref _isCategoryFloor, value); }
        }

        private ExternalCommandData _commandData;
        
        public List<Category> CategoriesCat { get; } = new List<Category> { };
        public Category CategoryCat { get; }
        public DelegateCommand ShowCategoryCat { get; }
        public DelegateCommand ShowWalls { get; }
        public DelegateCommand ShowFloors { get; }
        public Category SelectedCategoryCat { get; set; }
       

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            CategoriesCat = CategoryGeneralCat.GetCategories(commandData);
            //ShowCategoryCat = new DelegateCommand(OnShowCategoryCat);
            ShowWalls = new DelegateCommand(OnShowWalls);
            ShowFloors = new DelegateCommand(OnShowFloors);
        }

        private void OnShowFloors()
        {
            foreach (var category in CategoriesCat)
            {
                if (category.Name == "Перекрытия")
                {
                    UIDocument uidoc = _commandData.Application.ActiveUIDocument;
                    Document doc = uidoc.Document;
                    ICollection<ElementId> selectionElementIds = new List<ElementId>();
                    
                    if (IsCategoryFloor)
                    {

                        var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
                        .ToElements();

                        foreach (Element element in listOfElements)
                        {
                            if (element.Category != null && element.Category.Name.Equals("Перекрытия"))
                            {
                                selectionElementIds.Add(element.Id);
                            }
                        }
                        uidoc.Selection.SetElementIds(selectionElementIds);
                        //RaiseCloseRequest();
                    }
                    else if (!IsCategoryFloor)
                    {
                        List<ElementId> floorsToDeselect = new List<ElementId>();
                        uidoc.Selection.SetElementIds(floorsToDeselect);
                    }
                    //RaiseCloseRequest();
                }
            }
        }

        private void OnShowWalls()
        {           
            UIDocument uidoc = _commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ICollection<ElementId> selectionElementIds = new List<ElementId>();


            if (IsCategoryWall)
            {
                var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .ToElements();

                foreach (Element element in listOfElements)
                {
                    if (element.Category != null && element.Category.Name.Equals("Стены"))
                    {
                        selectionElementIds.Add(element.Id);
                    }
                }
                uidoc.Selection.SetElementIds(selectionElementIds);
                //RaiseCloseRequest();                
            }
            else if (!IsCategoryWall)
            {
                List<ElementId> wallsToDeselect = new List<ElementId>();
                uidoc.Selection.SetElementIds(wallsToDeselect);
            }            
        }
        //private void OnShowCategoryCat()
        //{
        //    UIDocument uidoc = _commandData.Application.ActiveUIDocument;
        //    Document doc = uidoc.Document;

        //    ICollection<ElementId> selectionElementIds = new List<ElementId>();
        //    var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
        //        .ToElements();

        //    foreach (Element element in listOfElements)
        //    {
        //        if (element.Category != null && element.Category.Name.Equals(SelectedCategoryCat.Name))
        //        {
        //            selectionElementIds.Add(element.Id);
        //        }
        //    }
        //    uidoc.Selection.SetElementIds(selectionElementIds);

        //    RaiseCloseRequest();
        //}

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

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
    //public static class CategoryGeneralCat
    //{
        
    //    //public static List<Category> GetCategories(ExternalCommandData commandData)
    //    //{
    //    //    UIDocument uidoc = commandData.Application.ActiveUIDocument;
    //    //    Document doc = uidoc.Document;
    //    //    List<Category> categoryNames = new List<Category>();

    //    //    List<string> categoryNamesStr = new List<string>();

    //    //    FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
    //    //    ICollection<Element> elementsInView = collector.ToElements();

    //    //    // Получение уникальных категорий элементов
    //    //    foreach (Element element in elementsInView)
    //    //    {
    //    //        if (element.Category != null)
    //    //        {
    //    //            string categoryName = element.Category.Name;
    //    //            Category category = element.Category;
    //    //            if (!categoryNamesStr.Contains(categoryName) && category != null)
    //    //            {
    //    //                categoryNamesStr.Add(categoryName);
    //    //                categoryNames.Add(category);
    //    //            }
    //    //        }
    //    //    }
    //    //    Comparison<Category> comparison = (x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    //    //    categoryNames.Sort(comparison);
    //    //    return categoryNames;
    //    //}
    //    //public static Category GetCategory(ExternalCommandData commandData)
    //    //{
    //    //    foreach (Category category in GetCategories(commandData))
    //    //    {
    //    //        CategoryModel categoryModel = new CategoryModel
    //    //        {
    //    //            Name = category.Name,
    //    //            IsSelected = false
    //    //        };
    //    //        return categoryModel;
    //    //    }
    //    //}

    //}
}
