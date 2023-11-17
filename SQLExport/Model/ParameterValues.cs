using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLExport.Model
{
    public class ParameterValues
    {
        public static List<string> GetParamValues(Element e)
        {
            // Two choices: 
            // Element.Parameters property -- Retrieves 
            // a set containing all the parameters.
            // GetOrderedParameters method -- Gets the 
            // visible parameters in order.

            var ps = e.GetOrderedParameters();
            var paramValues = new List<string>(ps.Count);

            // AsValueString displays the value as the 
            // user sees it. In some cases, the underlying
            // database value returned by AsInteger, AsDouble,
            // etc., may be more relevant.
            foreach (var p in ps)
            {
                if (p.StorageType == StorageType.Integer)
                {
                    paramValues.Add($"{p.Definition.Name} = {p.AsValueString()}");
                }
                else if (p.StorageType == StorageType.String)
                {
                    paramValues.Add($"{p.Definition.Name} = {p.AsString()}");
                }
                else if (p.StorageType == StorageType.Double)
                {
                    paramValues.Add($"{p.Definition.Name} = {p.AsValueString()}");
                }
                paramValues.Add($"{p.Definition.Name} = {p.AsValueString()}");

            }
            return paramValues;
        }
    }
}
