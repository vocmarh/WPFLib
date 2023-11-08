using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBox.Model
{
    public class FamilyInstanceUtils
    {
        public List<Element> GetColumnElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> fiElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(FamilyInstance))
            .Cast<FamilyInstance>()
            .ToList();

            string categoryStr = Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns).Name;

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals(categoryStr))
                {
                    fiElements.Add(element);
                }
            }

            return fiElements;
        }

        public List<Element> GetFramingElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> fiElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(FamilyInstance))
            .Cast<FamilyInstance>()
            .ToList();

            string categoryStr = Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming).Name;

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals(categoryStr))
                {
                    fiElements.Add(element);
                }
            }

            return fiElements;
        }

        public List<Element> GetDoorsElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> fiElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(FamilyInstance))
            .Cast<FamilyInstance>()
            .ToList();

            string categoryStr = Category.GetCategory(doc, BuiltInCategory.OST_Doors).Name;

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals(categoryStr))
                {
                    fiElements.Add(element);
                }
            }

            return fiElements;
        }

        public List<Element> GetWindowsElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> fiElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(FamilyInstance))
            .Cast<FamilyInstance>()
            .ToList();

            string categoryStr = Category.GetCategory(doc, BuiltInCategory.OST_Windows).Name;

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals(categoryStr))
                {
                    fiElements.Add(element);
                }
            }

            return fiElements;
        }
    }
}
