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

namespace SQLExport.ViewModel
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        SQLDBConnect sqlConnection = new SQLDBConnect();
        
        public DelegateCommand CreateDataTable { get; }
        public DelegateCommand ExportDataTable { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            CreateDataTable = new DelegateCommand(OnCreateDataTable);
            ExportDataTable = new DelegateCommand(OnExportDataTable);
        }
        
        private void OnExportDataTable()
        {                    
            
        }       

        private void OnCreateDataTable()
        {
            sqlConnection.ConnectDB();
            //Check if Doors table exists 
            bool doesExist = TableExists("testing", "Doors");

            if (doesExist)
            {
                TaskDialog.Show("SQL Table Error", "SQL table already exists");
            }
            else
            {
                try
                {
                    //SQL query to create door table
                    string tableQuery = "CREATE TABLE Doors" +
                       "(UniqueId varchar(255) NOT NULL PRIMARY KEY, Имя varchar(255)," +
                       "Level varchar(255), DateExport varchar(255))";

                    SqlCommand command = sqlConnection.Query(tableQuery);
                    command.ExecuteNonQuery();

                    TaskDialog.Show("Create SQL Table", "Doors table created");
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("SQL Error", ex.ToString());
                }
            }
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
