using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckBox.Model
{
    public class ElementData
    {
        public string ElementName { get; set; }
        public double Volume { get; set; }
        public string CategoryName { get; set; }
        public string Level {  get; set; }
        public double Area { get; set; }
        public string BoundaryRoom { get; set; }
        public double Length { get; set; }


    }
    

}
