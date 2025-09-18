using System.Data;
using System.Reflection;
using System.Text.Json;

namespace CoreLib.Application.Common.Utility
{
    public static class DBUtilities
    {
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        public static List<T> ConvertJsonDataTable<T>(DataTable dt)
        {
            List<T> data = [];
            foreach (DataRow row in dt.Rows)
            {
                var json = row[0]?.ToString();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    T[] item = JsonSerializer.Deserialize<T[]>(json);
                    data.AddRange(item);
                }
            }
            return data;
        }

        //private static T GetItem<T>(DataRow dr)
        //{
        //    Type temp = typeof(T);
        //    T obj = Activator.CreateInstance<T>();

        //    foreach (DataColumn column in dr.Table.Columns)
        //    {
        //        foreach (PropertyInfo pro in temp.GetProperties())
        //        {
        //            if (pro.Name == column.ColumnName)
        //            {
        //                pro.SetValue(obj, Convert.ToString(dr[column.ColumnName]), null); //dont add IformartProvider to Convert.ToString here
        //            }
        //        }
        //    }
        //    return obj;
        //}
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        // Handle DBNull values and type conversion
                        if (dr[column.ColumnName] != DBNull.Value)
                        {

                            pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], pro.PropertyType), null);
                        }
                        else
                        {
                            // Set default value for nullable types or null for reference types
                            pro.SetValue(obj, null, null);
                        }
                        break; // Found matching property, move to next column
                    }
                }
            }
            return obj;
        }
    }
}
