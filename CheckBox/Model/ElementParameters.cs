using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace CheckBox.Model
{
    public class ElementParameters
    {
        public List<string> GetParameterNames(Element element)
        {
            
            List<string> parameterNames = new List<string>();

            foreach (Parameter param in element.Parameters)
            {
                if (!parameterNames.Contains(param.Definition.Name))
                {
                    parameterNames.Add(param.Definition.Name);
                }
            }
            return parameterNames;
        }
    }
}