using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CheckBox.Model
{
    public class ProcessElements
    {
        public void GetProcessElements(List<Element> elements, ObservableCollection<ElementData> Data, ExternalCommandData commandData)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            foreach (Element element in elements)
            {
                try
                {                  
                    string elementName = element.Name;
                    string categoryName = element.Category.Name;
                    double volume = 0.0;
                    string level = string.Empty;

                    Parameter volPar = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                    if (volPar != null)
                    {
                        double metr3 = Math.Pow(0.3048, 3);
                        volume = volPar.AsDouble() * metr3;
                    }

                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Walls).Name))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                        if (levPar != null)
                        {
                            level = levPar.AsValueString();
                        }
                    }

                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Floors).Name))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.LEVEL_PARAM);
                        if (levPar != null)
                        {
                            level = levPar.AsValueString();
                        }
                    }

                    Data.Add(new ElementData
                    {
                        ElementName = elementName,
                        Volume = volume,
                        CategoryName = categoryName,
                        Level = level,
                    });
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.Message);
                }
            }
        }
    }
}
