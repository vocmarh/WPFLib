using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SQLExport.Model
{
    public class SaveParamsSQL
    {
        public void SaveSQLParams(List<string> paramList, string setQuery, string uniqueId, List<string> distictParams)
        {
            SQLDBConnect sqlConnection = new SQLDBConnect();
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
    }
}
