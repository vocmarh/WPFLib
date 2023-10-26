using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBox.Model
{
    public class FloorUtils
    {
        public ICollection<ElementId> GetElementIds(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ICollection<ElementId> selectionElementIds = new List<ElementId>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals("Floors"))
                {
                    selectionElementIds.Add(element.Id);
                }
            }
            //uidoc.Selection.SetElementIds(selectionElementIds);
            return selectionElementIds;
        }

        public List<string> GetElementName(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<string> selectionElementNames = new List<string>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals("Floors"))
                {
                    string elementName = element.Name.ToString();
                    selectionElementNames.Add(elementName);
                }
            }

            return selectionElementNames;
        }

        public List<Element> GetElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> floorElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals("Floors"))
                {
                    var floorElement = element as Floor;
                    floorElements.Add(floorElement);
                }
            }

            return floorElements;
        }
    }
}
