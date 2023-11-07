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

            
            string categoryName = string.Empty;

            List<Category> categoryNames = new List<Category>();
            List<string> categoryNamesStr = new List<string>();

            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            ICollection<Element> elementsInView = collector.ToElements();

            // Получение уникальных категорий элементов
            foreach (Element element in elementsInView)
            {
                if (element.Category != null)
                {
                    categoryName = element.Category.Name;
                    Category category = element.Category;
                    if (!categoryNamesStr.Contains(categoryName) && category != null)
                    {
                        categoryNamesStr.Add(categoryName);
                        categoryNames.Add(category);                        
                                                
                        Categories.Add(new CategoryModel
                        {
                            CategoryName = categoryName,
                            IsSelected = false,
                        });
                   
                    }
                }
            }
            
        }
    }
}
