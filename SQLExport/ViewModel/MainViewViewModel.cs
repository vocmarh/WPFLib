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

namespace SQLExport.ViewModel
{
    public class MainViewViewModel: INotifyPropertyChanged
    {
        private static readonly Array valueList = Enum.GetValues(typeof(BuiltInCategory));
        private static List<BuiltInCategory> builtInCatArray = valueList.OfType<BuiltInCategory>().ToList();
        private static readonly HashSet<string> checkedRowsString = new HashSet<string>();
        private ExternalCommandData _commandData;

        private Document _doc;

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
                        checkedRowsString.Add(category.CategoryName);
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
                string selectedCategory = category.CategoryName;

                foreach (var value in checkedRowsString)
                {
                    if (value == selectedCategory)
                    {
                        foreach (BuiltInCategory bic in builtInCatArray)
                        {
                            var cat = bic.ToString().Substring(bic.ToString().IndexOf("_") + 1);
                            if (cat == selectedCategory)
                            {
                                string selectedCat = projectName + "_" + cat;
                            }
                        }
                    }
                    
                }
            }

            
        }         

        private void OnCreateDataTable()
        {
            


            //Check if Doors table exists 
            //bool doesExist = TableExists("testing", "Doors");

            //if (doesExist)
            //{
            //    TaskDialog.Show("SQL Table Error", "SQL table already exists");
            //}
            //else
            //{
            //    try
            //    {
            //        //SQL query to create door table
            //        string tableQuery = "CREATE TABLE Doors" +
            //           "(UniqueId varchar(255) NOT NULL PRIMARY KEY, Имя varchar(255)," +
            //           "Level varchar(255), DateExport varchar(255))";

            //        SqlCommand command = sqlConnection.Query(tableQuery);
            //        command.ExecuteNonQuery();

            //        TaskDialog.Show("Create SQL Table", "Doors table created");
            //    }
            //    catch (Exception ex)
            //    {
            //        TaskDialog.Show("SQL Error", ex.ToString());
            //    }
            //}
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
