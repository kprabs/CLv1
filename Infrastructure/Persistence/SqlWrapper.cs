using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace CoreLib.Infrastructure.Persistence
{
    public static class SqlWrapper
    {
        /// <summary>
        /// Executes the given query using the SQL connection, then returns the
        /// results in a DataTable
        /// </summary>
        /// 

        public static DataTable ExecuteDataTable(string connString, string queryString, Dictionary<string, string> objParams, ILogger logger, bool IsTrueValue = false)
        {
            DataTable dataTable = new();

            using (SqlConnection conn = new(connString))
            {
                using (SqlDataAdapter da = new(queryString, conn))
                {

                    if (IsTrueValue)
                    {
                        try
                        {
                            conn.Open();
                            SqlCommand cmd = new(queryString, conn);
                            da.SelectCommand = cmd;
                            foreach (var param in objParams)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                            logger.LogInformation($"Sql Connection Established {conn.State == ConnectionState.Open}");
                            logger.LogInformation($"Sql Command Text::: " + cmd.CommandText);
                            logger.LogInformation($"Sql DA SELECT COMMAND::: " + da.SelectCommand);
                            logger.LogInformation($"Sql QUERY::: " + queryString);
                            da.Fill(dataTable);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError($"Sql Connection Error: {ex.Message}");
                        }
                        finally
                        {
                            //close and dispose connection
                            conn.Close();
                            da.Dispose();
                        }
                    }
                    else
                    {
                        try
                        {
                            foreach (var param in objParams)
                            {
                                queryString = queryString.Replace(param.Key, param.Value, StringComparison.InvariantCultureIgnoreCase);
                            }
                            da.SelectCommand.CommandText = queryString;
                            logger.LogInformation($"Sql QUERY::: " + queryString);
                            conn.Open();
                            logger.LogInformation($"Sql Connection Established {da.SelectCommand.Connection.State == ConnectionState.Open}");

                            da.Fill(dataTable);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError($"Sql Connection Error: {ex.Message}");

                        }
                    }
                }
            }

            return dataTable;
        }

        public static DataTable ExecuteDataTable(string connString, string queryString, ILogger logger)
        {
            DataTable dataTable = new();

            using (SqlConnection conn = new(connString))
            {
                using (SqlDataAdapter da = new(queryString, conn))
                {
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new(queryString, conn);
                        logger.LogInformation($"Sql QUERY::: " + queryString);
                        da.SelectCommand = cmd;
                        logger.LogInformation($"Sql Connection Established {da.SelectCommand.Connection.State == ConnectionState.Open}");

                        logger.LogInformation($"Sql Connection Established {conn.State == ConnectionState.Open}");

                        da.Fill(dataTable);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Sql Connection Error: {ex.Message}");
                    }
                    finally
                    {
                        //close and dispose connection
                        conn.Close();
                        da.Dispose();
                    }
                }
            }

            return dataTable;
        }

        public static DataTable ExecuteDataTable(string queryString, Dictionary<string, string> objParams, ILogger logger, bool IsTrueValue = false)
        {
            string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:AuthEntities").Value;

            return ExecuteDataTable(connString, queryString, objParams, logger);
        }
        public static DataTable ExecuteDataTable(string queryString, ILogger logger)
        {
            string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:AuthEntities").Value;

            return ExecuteDataTable(connString, queryString, logger);
        }
        public static DataSet ExecuteDataSet(List<string> queryStrings, Dictionary<string, string> objParams, ILogger logger)
        {
            DataSet dataSet = new();
            string connString = ConfigurationHelper.config.GetSection("ConnectionStrings:AuthEntities").Value;

            string queryString = String.Join("; ", queryStrings);
            try
            {
                using (SqlConnection conn = new(connString))
                {
                    conn.Open();

                    foreach (var param in objParams)
                    {
                        queryString = queryString.Replace(param.Key, param.Value);
                    }
                    logger.LogInformation($"Sql QUERY::: " + queryString);
                    using (SqlDataAdapter da = new(queryString, conn))
                    {
                        logger.LogInformation($"Sql Connection Established {da.SelectCommand.Connection.State == ConnectionState.Open}");


                        da.Fill(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Sql Connection Error: {ex.Message}");
            }

            return dataSet;
        }

        /// <summary>
        /// Executes the given query using the SQL connection, then returns the
        /// results transformed via the given function.
        /// </summary>
        public static IList<T> Query<T>(this SqlConnection connection, string query, Func<SqlDataReader, T> load, ILogger logger)
        {
            List<T> result = [];
            try
            {
                connection.Open();
                logger.LogInformation($"Sql QUERY::: "+ query);
                using (SqlCommand command = new(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        logger.LogInformation($"Sql Connection Established {connection.State == ConnectionState.Open}");

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result.Add(load(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Sql Connection Error: {ex.Message}");
            }
            return result;
        }
    }
}