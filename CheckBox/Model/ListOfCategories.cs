using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CheckBox.Model
{
    public class ListOfCategories
    {
        public void GetCategories(ExternalCommandData commandData, ObservableCollection<CategoryModel> Categories)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<BuiltInCategory> bicCat = Enum.GetValues(typeof(BuiltInCategory)).OfType<BuiltInCategory>().ToList();

            foreach (var bic in bicCat)
            {
                var cat = bic.ToString().Substring(bic.ToString().IndexOf("_") + 1);
                Categories.Add(new CategoryModel
                {
                    CategoryName = cat,
                    IsSelected = false,
                });
            }
            List<string> list = new List<string>();
        }
    }
}
