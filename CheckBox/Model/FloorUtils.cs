﻿using Autodesk.Revit.DB;
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
        public List<Element> GetElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> floorElements = new List<Element>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            string categoryStr = Category.GetCategory(doc, BuiltInCategory.OST_Floors).Name;

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals(categoryStr)) 
                {
                    var floorElement = element as Floor;
                    floorElements.Add(floorElement);
                }
            }

            return floorElements;
        }        
    }
}
