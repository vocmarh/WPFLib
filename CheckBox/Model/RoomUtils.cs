using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBox.Model
{
    public class RoomUtils
    {
        public List<Element> GetRoomElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> roomElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .OfClass(typeof(SpatialElement))
            .Cast<SpatialElement>()
            .ToList();

            string categoryStr = Category.GetCategory(doc, BuiltInCategory.OST_Rooms).Name;

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals(categoryStr))
                {
                    roomElements.Add(element);
                }
            }

            return roomElements;
        }
    }
}
