using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBox.Model
{
    public class GetElemByCat
    {
        public List<Element> GetElementsByCategoryName(string categoryName, ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            string categoryFloorStr = Category.GetCategory(doc, BuiltInCategory.OST_Floors).Name;
            if (categoryName == categoryFloorStr)
            {
                FloorUtils floorUtils = new FloorUtils();
                return floorUtils.GetElements(commandData);
            }

            string categoryWallStr = Category.GetCategory(doc, BuiltInCategory.OST_Walls).Name;
            if (categoryName == categoryWallStr)
            {
                WallUtils wallUtils = new WallUtils();
                return wallUtils.GetElements(commandData);
            }

            string categoryRebarStr = Category.GetCategory(doc, BuiltInCategory.OST_Rebar).Name;
            if (categoryName == categoryRebarStr)
            {
                RebarUtils rebarUtils = new RebarUtils();
                return rebarUtils.GetElements(commandData);
            }

            return new List<Element>();
        }
    }
}
