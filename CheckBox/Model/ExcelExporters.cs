using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Application = Microsoft.Office.Interop.Excel.Application;
using Autodesk.Revit.Attributes;

namespace CheckBox.Model
{
    public class ExcelExporters
    {
        public void GetElementToExcel(ObservableCollection<ElementData> Data)
        {
            try
            {
                Application excelApp = new Application();
                Workbook workbook = excelApp.Workbooks.Add();
                Worksheet worksheet = workbook.Worksheets[1];
                
                // Записываем заголовки столбцов в Excel
                worksheet.Cells[1, 1] = "Наименование элемента";
                worksheet.Cells[1, 2] = "Объем";
                worksheet.Cells[1, 3] = "Категория";
                worksheet.Cells[1, 4] = "Уровень";

                int row = 2;
                foreach (ElementData element in Data)
                {
                    worksheet.Cells[row, 1] = element.ElementName;
                    worksheet.Cells[row, 2] = element.Volume;
                    worksheet.Cells[row, 3] = element.CategoryName;
                    worksheet.Cells[row, 4] = element.Level;
                    row++;
                }

                var saveDialog = new SaveFileDialog
                {
                    OverwritePrompt = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = "All files (*.*)|*.*",
                    FileName = "Data.xlsx",
                    DefaultExt = ".xlsx"
                };

                string selectedFilePath = string.Empty;
                if (saveDialog.ShowDialog() == true)
                {
                    selectedFilePath = saveDialog.FileName;
                    workbook.SaveAs(selectedFilePath);

                    workbook.Close();
                    excelApp.Quit();

                    // Освободите ресурсы COM
                    Marshal.ReleaseComObject(worksheet);
                    Marshal.ReleaseComObject(workbook);
                    Marshal.ReleaseComObject(excelApp);
                }                    
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
            }

        }

    }
}
