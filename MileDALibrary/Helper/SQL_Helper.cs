using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace MileDALibrary.Helper
{
    public class SQL_Helper
    {
        private static string _connectionString = string.Empty;

        public static void SetConnectionString(string value)
        {
            _connectionString = value;

        }

        public enum ExecutionType { Query, Procedure };

        public static DataTable ExecuteSelect<T>(string query, List<DbParameter> sqlParams, ExecutionType executionType) where T : IDbConnection, new()
        {
            using (var sqlConnection = new T())
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.Connection.ConnectionString = _connectionString;
                    sqlCommand.CommandText = query;
                    sqlCommand.CommandTimeout = 500;

                    if (executionType == ExecutionType.Procedure)
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                    else
                        sqlCommand.CommandType = CommandType.Text;

                    if (sqlParams != null)
                    {
                        foreach (DbParameter sqlParameter in sqlParams)
                        {
                            sqlCommand.Parameters.Add(sqlParameter);
                        }
                    }
                    sqlCommand.Connection.Open();
                    var dataTable = new DataTable();

                    dataTable.BeginLoadData();
                    dataTable.Load(sqlCommand.ExecuteReader());

                    dataTable.EndLoadData();
                    sqlCommand.Connection.Close();

                    return dataTable;
                }
            }
        }


        public static Dictionary<string, dynamic> ExecuteNonQuery<T>(string query, List<DbParameter> dbParams, ExecutionType executionType) where T : IDbConnection, new()
        {
            int rowsaffected = 0;
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            using (var sqlConnection = new T())
            {
                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.Connection.ConnectionString = _connectionString;
                    sqlCommand.CommandText = query;
                    if (executionType == ExecutionType.Procedure)
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                    }
                    else
                        sqlCommand.CommandType = CommandType.Text;

                    if (dbParams != null)
                    {
                        foreach (DbParameter dbParameter in dbParams)
                        {
                            sqlCommand.Parameters.Add(dbParameter);
                        }
                    }

                    sqlCommand.Connection.Open();
                    rowsaffected = sqlCommand.ExecuteNonQuery();

                    result.Add("RowsAffected", rowsaffected);
                    if (dbParams != null)
                    {
                        foreach (DbParameter param in dbParams)
                        {
                            if (param.Direction == ParameterDirection.Output)
                            {
                                result.Add(param.ParameterName, param.Value);
                            }
                        }
                    }
                    sqlCommand.Parameters.Clear();
                    sqlCommand.Connection.Close();
                    return result;
                }
            }
        }



        public static List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            Type temp = typeof(T);
            PropertyInfo[] props = temp.GetProperties();
            string[] propsName = new string[props.Length];
            int i = 0;

            foreach (PropertyInfo pro in props)
            {
                var pInfo = typeof(T).GetProperty(pro.Name)
                      .GetCustomAttribute<ColumnAttribute>();
                propsName[i] = pInfo != null && !string.IsNullOrEmpty(pInfo.Name) ? pInfo.Name : pro.Name;
                i++;
            }

            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row, props, propsName);
                data.Add(item);
            }

            return data;
        }

        public static T GetItem<T>(DataRow dr, PropertyInfo[] props, string[] propsName)
        {
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                int i = 0;
                foreach (PropertyInfo pro in props)
                {
                    if (string.Compare(propsName[i], column.ColumnName, true, CultureInfo.CurrentCulture) == 0)
                    {
                        if (dr[column.ColumnName] != DBNull.Value)
                            pro.SetValue(obj, dr[column.ColumnName], null);
                        else
                            pro.SetValue(obj, null, null);
                        break;
                    }
                    i++;
                }
            }
            return obj;
        }
    }
}