﻿using Autodesk.Revit.DB;
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

            if (categoryName == Category.GetCategory(doc, BuiltInCategory.OST_Floors).Name)
            {
                FloorUtils floorUtils = new FloorUtils();
                return floorUtils.GetElements(commandData);
            }

            //if (categoryName == Category.GetCategory(doc, BuiltInCategory.OST_Walls).Name)
            //{
            //    WallUtils wallUtils = new WallUtils();
            //    return wallUtils.GetElements(commandData);
            //}

            //if (categoryName == Category.GetCategory(doc, BuiltInCategory.OST_Rebar).Name)
            //{
            //    RebarUtils rebarUtils = new RebarUtils();
            //    return rebarUtils.GetElements(commandData);
            //}

            return new List<Element>();
        }
    }
}