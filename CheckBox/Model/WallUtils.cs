﻿using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CheckBox.Model
{
    public class WallUtils
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
                if (element.Category != null && element.Category.Name.Equals("Стены"))
                {
                    selectionElementIds.Add(element.Id);
                }
            }
            //uidoc.Selection.SetElementIds(selectionElementIds);
            return selectionElementIds;
        }

        public List<string> GetElementName (ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;         
            List<string> selectionElementNames = new List<string>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals("Стены"))
                {
                    string elementName = element.Name.ToString();
                    selectionElementNames.Add(elementName);
                }
            }
            
            return selectionElementNames;
        }
        public double GetVolume(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            double volume = 0;
            //List<double> volumes = new List<double>();
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals("Стены"))
                {
                    Parameter volPar = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                    volume = volPar.AsDouble(); 
                    //volumes.Add(volume);
                }
            }

            return volume;
        }
        public List<Element> GetElements(ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<Element> wallElements = new List<Element>();   
            var listOfElements = new FilteredElementCollector(doc, doc.ActiveView.Id)
            .ToElements();

            foreach (Element element in listOfElements)
            {
                if (element.Category != null && element.Category.Name.Equals("Стены"))
                {
                    var wallElement = element as Wall;
                    wallElements.Add(wallElement);
                }
            }
            //uidoc.Selection.SetElementIds(selectionElementIds);
            return wallElements;
        }
    }
}