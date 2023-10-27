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
    public class ProcessElements
    {
        public void GetProcessElements(List<Element> elements, ObservableCollection <ElementData> Data)
        {
            foreach (Element element in elements)
            {
                try
                {
                    string elementName = element.Name;
                    string categoryName = element.Category.Name;
                    double volumeMeters = 0.0;
                    string level = string.Empty;

                    if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls || element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors)
                    {
                        Parameter volPar = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                        if (volPar != null)
                        {
                            double volume = volPar.AsDouble();
                            volumeMeters = UnitUtils.ConvertFromInternalUnits(volume, DisplayUnitType.DUT_CUBIC_METERS);
                        }
                    }

                    if (element.Category.Name.Equals("Перекрытия"))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.LEVEL_PARAM);
                        if (levPar != null)
                        {
                            level = levPar.AsValueString();
                        }
                    }


                    if (element.Category.Name.Equals("Стены"))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                        if (levPar != null)
                        {
                            level = levPar.AsValueString();
                        }
                    }

                    Data.Add(new ElementData
                    {
                        ElementName = elementName,
                        Volume = volumeMeters,
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
