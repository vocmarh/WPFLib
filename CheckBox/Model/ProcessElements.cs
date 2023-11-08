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
                    string elementName = string.Empty;
                    string categoryName = string.Empty;
                    double volume = 0.0;
                    string level = string.Empty;
                    double area = 0.0;
                    string boundaryRoom = string.Empty;
                    string length = string.Empty;
                    string width = string.Empty;
                    string height = string.Empty;
                    string workSet = string.Empty;
                    double roomArea = 0.0;

                    Parameter elemPar = element.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM);
                    if (elemPar != null)
                    {
                        elementName = elemPar.AsValueString();
                    }

                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Rooms).Name))
                    {
                        Parameter elemPar1 = element.get_Parameter(BuiltInParameter.ROOM_NAME);
                        if (elemPar1 != null)
                        {
                            elementName = elemPar1.AsString();
                        }
                    }

                    Parameter volPar = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                    if (volPar != null)
                    {
                        double metr3 = Math.Pow(0.3048, 3);
                        volume = volPar.AsDouble() * metr3;
                    }

                    Parameter areaPar = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
                    if (areaPar != null)
                    {
                        double metr2 = Math.Pow(0.3048, 2);
                        area = areaPar.AsDouble() * metr2;
                    }

                    Parameter roomAreaPar = element.get_Parameter(BuiltInParameter.ROOM_AREA);
                    if (roomAreaPar != null)
                    {
                        double metr2 = Math.Pow(0.3048, 2);
                        roomArea = roomAreaPar.AsDouble() * metr2;
                    }

                    Parameter boundaryPar = element.get_Parameter(BuiltInParameter.WALL_ATTR_ROOM_BOUNDING);
                    if (boundaryPar != null)
                    {
                        boundaryRoom = boundaryPar.AsValueString();
                    }

                    Parameter lengthPar = element.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                    if (lengthPar != null)
                    {
                        length = lengthPar.AsValueString();
                    }

                    Parameter workSetPar = element.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM);
                    if (workSetPar != null)
                    {
                        workSet = workSetPar.AsValueString();
                    }

                    #region LevelPar
                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Walls).Name))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT);
                        if (levPar != null)
                        {
                            level = levPar.AsValueString();
                        }
                    }

                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_StructuralColumns).Name))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
                        if (levPar != null)
                        {
                            level = levPar.AsValueString();
                        }
                    }

                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming).Name))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM);
                        if (levPar != null)
                        {
                            level = levPar.AsValueString();
                        }
                    }

                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Doors).Name) || element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Windows).Name))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
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

                    if (element.Category.Name.Equals(Category.GetCategory(doc, BuiltInCategory.OST_Rooms).Name))
                    {
                        Parameter levPar = element.get_Parameter(BuiltInParameter.LEVEL_NAME);
                        if (levPar != null)
                        {
                            level = levPar.AsString();
                        }
                    }
                    #endregion

                    ElementType type = doc.GetElement(element.GetTypeId()) as ElementType;
                    if (type != null)
                    {
                        Parameter widthPar = type.get_Parameter(BuiltInParameter.FURNITURE_WIDTH);
                        if (widthPar != null)
                        {
                            width = widthPar.AsValueString();
                        }

                        Parameter heightPar = type.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM);
                        if (heightPar != null)
                        {
                            height = heightPar.AsValueString();
                        }
                    }                    

                    Data.Add(new ElementData
                    {
                        ElementName = elementName,
                        Volume = volume,
                        CategoryName = categoryName,
                        Level = level,
                        Area = area,
                        BoundaryRoom = boundaryRoom,
                        Length = length,
                        Width = width,
                        Height = height,
                        WorkSet = workSet,
                        RoomArea = roomArea,
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
