using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLExport.View;

namespace SQLExport
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var window = new MainView(commandData);

            //window.Width = 500;
            //window.Height = 150;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            window.Show();

            return Result.Succeeded;
        }

    }
}
