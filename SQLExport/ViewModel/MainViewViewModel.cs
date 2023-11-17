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
using System.Windows.Data;

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
        //private string _searchText;
        //public string SearchText
        //{
        //    get { return _searchText; }
        //    set { _searchText = value; NotifyPropertyChanged(nameof(SearchText)); }
        //}
        //private string pattern;
        //public string Pattern
        //{
        //    get { return pattern; }
        //    set { pattern = value; NotifyPropertyChanged(nameof(Pattern));
        //        FilterCategories(null); }
        //}
        //private ICollectionView categoriesView;
        //public ICollectionView CategoriesView
        //{
        //    get { return categoriesView; }
        //    set { categoriesView = value; NotifyPropertyChanged(nameof(CategoriesView)); }
        //}
        private void NotifyPropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateCommand ExportDataTable { get; }
        public DelegateCommand CheckBoxIsChecked { get; }
        public DelegateCommand AddChosenCat { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            ExportDataTable = new DelegateCommand(OnExportDataTable);            
            AddChosenCat = new DelegateCommand(OnAddChosenCat);
            Categories = new ObservableCollection<CategoryModel>();

            ListOfCategories listOfCategories = new ListOfCategories();
            listOfCategories.GetCategories(_commandData, Categories);
            //CategoriesView = CollectionViewSource.GetDefaultView(Categories);
            //CategoriesView.Filter = FilterCategories;
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
                        checkedRowsString.Add(catName);
                    }
                }
            }
        }

        private void OnExportDataTable()
        {
                UIDocument uidoc = _commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Document arDoc = doc.Application.Documents.OfType<Document>().Where(x=>x.Title.Contains("АР")).FirstOrDefault();
            Document vkDoc = doc.Application.Documents.OfType<Document>().Where(x => x.Title.Contains("ВК")).FirstOrDefault();
            Document ovDoc = doc.Application.Documents.OfType<Document>().Where(x => x.Title.Contains("ОВ")).FirstOrDefault();
            if (arDoc == null)
            {
                TaskDialog.Show("Ошибка", "Не найден АР файл");
            }

            Dictionary<string, Dictionary<string, List<string>>> CatElementParameterValues
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

                                //Получаем название таблицы в виде: _Дата

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

                                CatElementParameterValues.Add(selectedCat, new Dictionary<string, List<string>>());

                                //Получаю все ID выбранных категорий
                                List<ElementId> bicID = new List<ElementId>();
                                bicID.Add(new ElementId((int)bic));

                                IList<ElementFilter> a = new List<ElementFilter>();
                                a.Add(new ElementCategoryFilter(bic));

                                var categoryFilter = new LogicalOrFilter(a);

                                //Apply the filter to the elements in the active document
                                ElementCategoryFilter filter = new ElementCategoryFilter(bic);
                                List<Element> allElements = new List<Element>();

                                //получаю коллектор с элементами выбранных категорий
                                //var els = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                //        .WhereElementIsNotElementType()
                                //        .WhereElementIsViewIndependent()
                                //        .WherePasses(categoryFilter);
                                List<Element> elsOV = new List<Element>();

                                var elsAr = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                        .WhereElementIsNotElementType()
                                        .WhereElementIsViewIndependent()
                                        .WherePasses(categoryFilter)
                                        .ToElements()
                                        .ToList();
                                if (ovDoc != null)
                                {
                                    elsOV = new FilteredElementCollector(ovDoc, ovDoc.ActiveView.Id)
                                        .WhereElementIsNotElementType()
                                        .WhereElementIsViewIndependent()
                                        .WherePasses(categoryFilter)
                                        .ToElements()
                                        .ToList();
                                }
                                
                                //var elsVk = new FilteredElementCollector(doc, doc.ActiveView.Id)
                                //        .WhereElementIsNotElementType()
                                //        .WhereElementIsViewIndependent()
                                //        .WherePasses(categoryFilter)
                                //        .ToElements()
                                //        .ToList();
                                allElements.AddRange(elsAr);
                                allElements.AddRange(elsOV);
                                //allElements.AddRange(elsVk);

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

                                foreach (Element ele in allElements)
                                {
                                    var elementCat = ele.Category;
                                    if (null == elementCat)
                                    {
                                        Debug.Print("element {0} {1} has null category", ele.Id, ele.Name);
                                        continue;
                                    }

                                    //Gets all the paramaters with extra family type and type id, etc.
                                    List<string> paramValues = ParameterValues.GetParamValues(ele);
                                    
                                    //Add family name to param values
                                    var familyName = "FamilyName = " + ele.Name;
                                    paramValues.Add(familyName);

                                    //Add element id name to param values
                                    var elementId = "ElementId = " + ele.Id.IntegerValue;
                                    paramValues.Add(elementId);

                                    //Add element id name to param values
                                    var title = "Title = " + projectName;
                                    paramValues.Add(title);

                                    var dateOfProject = "Date = " + date;
                                    paramValues.Add(dateOfProject);

                                    string uid = ele.UniqueId;

                                    //Add uid, param_values and the category to map_cat_to_uid_to_param_values 
                                    CatElementParameterValues[selectedCat].Add(uid, paramValues);

                                    //Push params to list to get distict list
                                    for (var i = 0; i < paramValues.Count; i++)
                                    {
                                        List<string> elementParams = new List<string>(paramValues[i].Split(new string[] { " = " }, StringSplitOptions.None));
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

                                        if(!paramListParams.Contains(elementParam))
                                        {
                                            paramListParams.Add(elementParam);
                                        }

                                        paramListParams.Add(elementParam);
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

                                foreach (var dictPair in CatElementParameterValues)
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
                                                    SaveParamsSQL saveParamsSQL = new SaveParamsSQL();
                                                    saveParamsSQL.SaveSQLParams(paramList, setQuery, uniqueId, distictParams);
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
                                                    SaveParamsSQL saveParamsSQL = new SaveParamsSQL();
                                                    saveParamsSQL.SaveSQLParams(paramList, setQuery, uniqueId, distictParams);
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
                                CatElementParameterValues.Clear();
                            }
                        }
                    }                    
                }
            }
            MessageBox.Show("Data successfully exported");
            checkedRowsString.Clear();
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

        //private bool FilterCategories(object obj)
        //{
        //    if (obj is CategoryModel category)
        //    {
        //        return string.IsNullOrEmpty(SearchText) || category.CategoryName.ToLower().Contains(SearchText.ToLower());
        //    }
        //    return false;
        //}
                
    }
}
