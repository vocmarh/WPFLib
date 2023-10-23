using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBox
{
    //public class CategoriesViewModel
    //{
    //    public ObservableCollection<CategoryModel> Categories { get; set; }

    //    public CategoriesViewModel(ExternalCommandData commandData)
    //    {
    //        UIDocument uidoc = commandData.Application.ActiveUIDocument;
    //        Document doc = uidoc.Document;
    //        ICollection<ElementId> selectionElementIds = new List<ElementId>();
    //        var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
    //        .ToElements();

    //        foreach (Element element in listOfElements)
    //        {
    //            if (element.Category != null && element.Category.Name.Equals(SelectedCategoryCat.Name))
    //            {
    //                selectionElementIds.Add(element.Id);
    //            }
    //        }
    //    }
    //}
}
