using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLExport.Model;
using System.Collections.ObjectModel;
using CheckBox.Model;
using Autodesk.Revit.DB;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows;
using Prism.Services.Dialogs;
using System.Text.RegularExpressions;

namespace SQLExport.ViewModel
{
    public class MainViewViewModel: INotifyPropertyChanged
    {
        private Document _doc;
        private static readonly Array valueList = Enum.GetValues(typeof(BuiltInCategory));
        private static List<BuiltInCategory> builtInCatArray = valueList.OfType<BuiltInCategory>().ToList();
        private static readonly HashSet<string> checkedRowsString = new HashSet<string>();
        private ExternalCommandData _commandData;       

        private bool isSelected = true;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; NotifyPropertyChanged(nameof(IsSelected)); }
        }
        SQLDBConnect sqlConnection = new SQLDBConnect();

        private ObservableCollection<CategoryModel> categories;
        public ObservableCollection<CategoryModel> Categories
        {
            get => categories;
            set { categories = value; NotifyPropertyChanged(nameof(Categories)); }
        }
        private void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }

        public DelegateCommand ExportDataTable { get; }
        public DelegateCommand AddChosenCat { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;

            ExportDataTable = new DelegateCommand(OnExportDataTable);
            AddChosenCat = new DelegateCommand(OnAddChosenCat);

            Categories = new ObservableCollection<CategoryModel>();
            ListOfCategories listOfCategories = new ListOfCategories();
            listOfCategories.GetCategories(_commandData, Categories);

            
        }

        private void OnAddChosenCat()
        {
            if (isSelected)
            {
                foreach (var category in Categories)
                {
                    if (category.IsSelected)
                    {
                        string catName = category.CategoryName;
                        catName = string.Join("", catName.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                        checkedRowsString.Add(catName);
                    }
                }
            }
        }

        private void OnExportDataTable()
        {
            UIDocument uidoc = _commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Dictionary<string, Dictionary<string, List<string>>> map_cat_to_uid_to_param_values
             = new Dictionary<string, Dictionary<string, List<string>>>();

            string projectName = doc.Title;
            sqlConnection.ConnectDB();            

            foreach (var category in Categories)
            {
                string numberSymbol = "\u2116";
                string angleSymbol = "\u00B0";
                string selectedCategory = category.CategoryName;
                selectedCategory = string.Join("", selectedCategory.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                foreach (var value in checkedRowsString)
                {
                    if (value == selectedCategory)
                    {
                        foreach (BuiltInCategory bic in builtInCatArray)
                        {
                            var cat = bic.ToString().Substring(bic.ToString().IndexOf("_") + 1);
                            if (cat == selectedCategory)
                            {
                                //Добавляю параметр даты в param values                                                                 
                                string date = string.Empty;
                                string docPath = doc.PathName;
                                string pattern = @"(\d{2}\.\d{2}\.\d{4})";

                                Match match = Regex.Match(docPath, pattern);

                                if (match.Success)
                                {
                                    date = match.Groups[1].Value;
                                }
                                else
                                {
                                    Console.WriteLine("Дата не найдена.");
                                }

                                //Получаем название таблицы в виде: НазваниеПроекта_Перекрытия
                                //string selCat = cat;
                                //StringBuilder sb = new StringBuilder(selCat);
                                //sb.Insert(2, date);
                                //string selectedCat = sb.ToString();
                                string selectedCat = cat + "_" + date;
                                
                                selectedCat = string.Join("", selectedCat.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                                if (selectedCat.Contains("-"))
                                {
                                    selectedCat = selectedCat.Replace("-", "_");
                                }

                                //Remove unwanted characters from project name
                                if (selectedCat.Contains("~"))
                                {
                                    selectedCat = selectedCat.Replace("~", "_");
                                }
                                if (selectedCat.Contains("["))
                                {
                                    selectedCat = selectedCat.Replace("[", string.Empty);
                                }
                                if (selectedCat .Contains("]"))
                                {
                                    selectedCat = selectedCat.Replace("]", string.Empty);
                                }
                                if (selectedCat.Contains("{"))
                                {
                                    selectedCat = selectedCat.Replace("{", string.Empty);
                                }
                                if (selectedCat.Contains("}"))
                                {
                                    selectedCat = selectedCat.Replace("}", string.Empty);
                                }
                                if (selectedCat.Contains("("))
                                {
                                    selectedCat = selectedCat.Replace("(", string.Empty);
                                }
                                if (selectedCat.Contains(")"))
                                {
                                    selectedCat = selectedCat.Replace(")", string.Empty);
                                }
                                if (selectedCat.Contains("."))
                                {
                                    selectedCat = selectedCat.Replace(".", string.Empty);
                                }                                

                                map_cat_to_uid_to_param_values.Add(selectedCat, new Dictionary<string, List<string>>());

                                //Получаю все ID выбранных категорий
                                List<ElementId> bicID = new List<ElementId>();
                                bicID.Add(new ElementId((int)bic));

                                IList<ElementFilter> a = new List<ElementFilter>();
                                a.Add(new ElementCategoryFilter(bic));

                                var categoryFilter = new LogicalOrFilter(a);

                                //Apply the filter to the elements in the active document
                                ElementCategoryFilter filter = new ElementCategoryFilter(bic);

                                //получаю коллектор с элементами выбранных категорий
                                var els = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                        .WhereElementIsNotElementType()
                                        .WhereElementIsViewIndependent()
                                        .WherePasses(categoryFilter);

                                List<string> paramListParams = new List<string>();

                                //Check if table is created if so just save values.
                                bool doesExist = TableExists("testing", selectedCat);

                                //If table exists , drop the table and reacreate it with new data
                                //This is a way to replace the old data
                                if (doesExist)
                                {
                                    SqlCommand command = sqlConnection.Query("DROP TABLE " + selectedCat);
                                    command.ExecuteNonQuery();
                                }

                                foreach (Element ele in els)
                                {
                                    var elementCat = ele.Category;
                                    if (null == elementCat)
                                    {
                                        Debug.Print("element {0} {1} has null category", ele.Id, ele.Name);
                                        continue;
                                    }

                                    //Gets all the paramaters with extra family type and type id, etc.
                                    List<string> param_values = GetParamValues(ele);

                                    //Add family name to param values
                                    var FamilyName = "FamilyName = " + ele.Name;
                                    param_values.Add(FamilyName);

                                    //Add element id name to param values
                                    var ElementId = "ElementId = " + ele.Id.IntegerValue;
                                    param_values.Add(ElementId);

                                    
                                    var dateOfProject = "Date = " + date;
                                    param_values.Add(dateOfProject);

                                    string uid = ele.UniqueId;

                                    //Add uid, param_values and the category to map_cat_to_uid_to_param_values 
                                    map_cat_to_uid_to_param_values[selectedCat].Add(uid, param_values);

                                    //Push params to list to get distict list
                                    for (var i = 0; i < param_values.Count; i++)
                                    {
                                        List<string> elementParams = new List<string>(param_values[i].Split(new string[] { " = " }, StringSplitOptions.None));
                                        string elementParam = string.Join("", elementParams[0].Split(default(string[]), StringSplitOptions.None));

                                        if (elementParam.Contains("-")) 
                                        {
                                            elementParam = elementParam.Replace("-", "_");
                                        }

                                        if (elementParam.Contains(numberSymbol))
                                        {
                                            elementParam = elementParam.Replace(numberSymbol, "");
                                        }

                                        if (elementParam.Contains(angleSymbol))
                                        {
                                            elementParam = elementParam.Replace(angleSymbol, "");
                                        }

                                        if (elementParam.Contains("/"))
                                        {
                                            elementParam = elementParam.Replace("/", "_");
                                        }


                                        if (elementParam.Contains("("))
                                        {
                                            elementParam = elementParam.Replace("(", "_");
                                        }

                                        if (elementParam.Contains(")"))
                                        {
                                            elementParam = elementParam.Replace(")", "_");
                                        }

                                        if (elementParam.Contains(" "))
                                        {
                                            elementParam = elementParam.Replace(" ", "_");
                                        }

                                        if (elementParam.Contains("1"))
                                        {
                                            elementParam = elementParam.Replace("1", "_");
                                        }


                                        if (elementParam.Contains("."))
                                        {
                                            elementParam = elementParam.Replace(".", "_");
                                        }

                                        if (elementParam.Contains(":"))
                                        {
                                            elementParam = elementParam.Replace(":", "_");
                                        }

                                        //if (elementParam.Contains("ON"))
                                        //{
                                        //    elementParam = elementParam.Replace("ON", "on" + "_" + cat);
                                        //}

                                        //if (elementParam.Contains("Select"))
                                        //{
                                        //    elementParam = elementParam.Replace("Select", "Select" + "_" + cat);
                                        //}

                                        //if (elementParam.Contains("Insert"))
                                        //{
                                        //    elementParam = elementParam.Replace("Insert", "Insert" + "_" + cat);
                                        //}

                                        //if (elementParam.Contains("Drop"))
                                        //{
                                        //    elementParam = elementParam.Replace("Drop", "Drop" + "_" + cat);
                                        //}

                                        //if (elementParam.Contains("Create"))
                                        //{
                                        //    elementParam = elementParam.Replace("Create", "Create" + "_" + cat);
                                        //}

                                        //if (elementParam.Contains("Into"))
                                        //{
                                        //    elementParam = elementParam.Replace("Into", "Into" + "_" + cat);
                                        //}

                                        //if (elementParam.Contains("Table"))
                                        //{
                                        //    elementParam = elementParam.Replace("Table", "Table" + "_" + cat);
                                        //}

                                        if(!paramListParams.Contains(elementParam))
                                        {
                                            paramListParams.Add(elementParam);
                                        }

                                        //paramListParams.Add(elementParam);
                                    }
                                }

                                //Get distinct parameters from paramListParams
                                List<string> distictParams = paramListParams.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                                

                                //Declare string builders 
                                StringBuilder sqlCreateTable = new StringBuilder("CREATE TABLE " + selectedCat + "(UniqueId varchar(255) NOT NULL PRIMARY KEY");
                                StringBuilder sqlSaveData = new StringBuilder("INSERT INTO " + selectedCat + "(UniqueId");
                                StringBuilder sqlSaveDataValues = new StringBuilder("VALUES" + "(@param1");

                                for (var i = 0; i < distictParams.Count; i++)
                                {
                                    if (i == distictParams.Count - 1)
                                    {
                                        sqlCreateTable.Append(", " + distictParams[i] + " varchar(255))");
                                        sqlSaveData.Append(", " + distictParams[i] + ") ");
                                        sqlSaveDataValues.Append($", @param{i + 2})");
                                    }
                                    else
                                    {
                                        sqlCreateTable.Append(", " + distictParams[i] + " varchar(255)");
                                        sqlSaveData.Append(", " + distictParams[i]);
                                        sqlSaveDataValues.Append($", @param{i + 2}");
                                    }
                                }
                                //table and set query strings
                                string tableQuery = sqlCreateTable.ToString();
                                string setQuery = sqlSaveData.Append(sqlSaveDataValues.ToString()).ToString();

                                foreach (var dictPair in map_cat_to_uid_to_param_values)
                                {
                                    if (dictPair.Value.Count > 0)
                                    {
                                        foreach (var innerPair in dictPair.Value)
                                        {
                                            string uniqueId = innerPair.Key;
                                            List<string> paramList = innerPair.Value;

                                            //Check if table is created if so just save values.
                                            doesExist = TableExists("testing", selectedCat);

                                            if (doesExist)
                                            {
                                                Debug.WriteLine(selectedCat + "table already exists");

                                                try
                                                {
                                                    SaveParamsSQL(paramList, setQuery, uniqueId, distictParams);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.ToString());
                                                }

                                            }
                                            else
                                            {
                                                try
                                                {
                                                    //Create sql table
                                                    SqlCommand command = sqlConnection.Query(tableQuery);
                                                    command.ExecuteNonQuery();
                                                    SaveParamsSQL(paramList, setQuery, uniqueId, distictParams);
                                                }
                                                catch (Exception ex)
                                                {
                                                    MessageBox.Show(ex.ToString());
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //MessageBox.Show("No data was found!");
                                        Debug.Print("No data was found!");
                                    }

                                }
                                map_cat_to_uid_to_param_values.Clear();
                            }
                        }
                    }                    
                }
            }
            MessageBox.Show("Data successfully exported");
            checkedRowsString.Clear();
        }
        private static List<string> GetParamValues(Element e)
        {
            // Two choices: 
            // Element.Parameters property -- Retrieves 
            // a set containing all the parameters.
            // GetOrderedParameters method -- Gets the 
            // visible parameters in order.

            var ps = e.GetOrderedParameters();
            var param_values = new List<string>(ps.Count);

            // AsValueString displays the value as the 
            // user sees it. In some cases, the underlying
            // database value returned by AsInteger, AsDouble,
            // etc., may be more relevant.
            foreach (var p in ps)
            {
                if(p.StorageType == StorageType.Integer)
                {
                    param_values.Add($"{p.Definition.Name} = {p.AsValueString()}");
                }
                else if (p.StorageType == StorageType.String)
                {
                    param_values.Add($"{p.Definition.Name} = {p.AsString()}");
                }
                param_values.Add($"{p.Definition.Name} = {p.AsValueString()}");

            }
            return param_values;
        }

        private void SaveParamsSQL(List<string> paramList, string setQuery, string uniqueId, List<string> distictParams)
        {
            SqlCommand command = sqlConnection.Query(setQuery);
            command.Parameters.AddWithValue($"@param1", uniqueId);

            //parameters from element 
            List<string> paramListParams = new List<string>();

            //get all params from wall
            for (var i = 0; i < paramList.Count; i++)
            {
                List<string> elementParams = new List<string>(paramList[i].Split(new string[] { " = " }, StringSplitOptions.None));
                string elementParam = string.Join("", elementParams[0].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                if (elementParam.Contains("-"))
                {
                    elementParam = elementParam.Replace("-", string.Empty);

                }

                paramListParams.Add(elementParam);

            }

            //remove duplicates in param list if any, try hash set or distinct instead of iterating.
            for (int i = 0; i < paramList.Count - 1; i++)
            {
                for (int j = i + 1; j < paramList.Count; j++)
                {

                    List<string> elementParams = new List<string>(paramList[i].Split(new string[] { " = " }, StringSplitOptions.None));
                    string elementParam = string.Join("", elementParams[0].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    List<string> elementParamsJ = new List<string>(paramList[j].Split(new string[] { " = " }, StringSplitOptions.None));
                    string elementParamJ = string.Join("", elementParamsJ[0].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    if (elementParam == elementParamJ)
                    {
                        paramList.RemoveAt(j);
                        paramListParams.RemoveAt(j);
                    }

                }
            }

            //Loop through all distinct parameters
            for (var i = 0; i < distictParams.Count; i++)
            {
                //Get the index of the paramListParams parameter that is equal to the distinct Parameter 
                int index = paramListParams.FindIndex(a => a == distictParams[i]);

                if (index >= 0)
                {
                    try
                    {
                        List<string> elementParams = new List<string>(paramList[index].Split(new string[] { " = " }, StringSplitOptions.None));
                        string elementParamValue = string.Join("", elementParams[1].Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                        //Remove unwanted parameters
                        switch (elementParamValue)
                        {
                            case var s when elementParamValue.Contains("°"):
                                elementParamValue = elementParamValue.Replace("°", string.Empty);
                                break;
                            case var s when elementParamValue.Contains("m³"):
                                elementParamValue = elementParamValue.Replace("m³", string.Empty);
                                break;
                            case var s when elementParamValue.Contains("<None>"):
                                elementParamValue = elementParamValue.Replace("<None>", string.Empty);
                                break;
                            case var s when elementParamValue.Contains("m²"):
                                elementParamValue = elementParamValue.Replace("m²", string.Empty);
                                break;
                        }

                        command.Parameters.AddWithValue($"@param{i + 2}", elementParamValue);

                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    try
                    {
                        command.Parameters.AddWithValue($"@param{i + 2}", "");
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }

            }

            command.ExecuteNonQuery();
        }       

        private bool TableExists(string database, string name)
        {
            
            try
            {
                //SQL query to check if doors table exists
                string existsQuery = "select case when exists((select * FROM [" + database + "].sys.tables " +
                    "WHERE name = '" + name + "')) then 1 else 0 end";

                SqlCommand command = sqlConnection.Query(existsQuery);

                //If value is 1 table exists if 0 , table doesnt exist
                return (int)command.ExecuteScalar() == 1;
            }
            catch (Exception err)
            {
                TaskDialog.Show("Error", err.ToString());
                return true;
            }
        }       

        public event EventHandler HideRequest;

        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;

        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CloseRequest;
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
