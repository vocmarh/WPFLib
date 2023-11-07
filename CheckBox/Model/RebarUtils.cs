using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBox.Model
{
    public class RebarUtils
    {
        public List<Element> GetElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> rebarElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            string categoryStr = Category.GetCategory(doc, BuiltInCategory.OST_Rebar).Name;

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals(categoryStr))
                {
                    rebarElements.Add(element);
                }
            }

            return rebarElements;
        }
    }
}
