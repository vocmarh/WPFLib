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
    public static class CategoryGeneralCat
    {
        public static List<Category> GetCategories(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Category> categoryNames = new List<Category>();

            List<string> categoryNamesStr = new List<string>();

            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            ICollection<Element> elementsInView = collector.ToElements();

            // Получение уникальных категорий элементов
            foreach (Element element in elementsInView)
            {
                if (element.Category != null)
                {
                    string categoryName = element.Category.Name;
                    Category category = element.Category;
                    if (!categoryNamesStr.Contains(categoryName) && category != null)
                    {
                        categoryNamesStr.Add(categoryName);
                        categoryNames.Add(category);
                    }
                }
            }
            Comparison<Category> comparison = (x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
            categoryNames.Sort(comparison);
            return categoryNames;
        }
    }
}
