using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace HumanMobility.Models.Services
{
    public class BulkCommands
    {
        public static void StoreActivities(IEnumerable<ActivityViewModel> activities)
        {
            string connectionString = GetConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var bulkCopy = new SqlBulkCopy(connection) {DestinationTableName = "dbo.Activities"};

                bulkCopy.ColumnMappings.Add("ID", "ID");
                bulkCopy.ColumnMappings.Add("UserId", "UserId");
                bulkCopy.ColumnMappings.Add("SaveTime", "SaveTime");
                bulkCopy.ColumnMappings.Add("X", "X");
                bulkCopy.ColumnMappings.Add("Y", "Y");
                bulkCopy.ColumnMappings.Add("Z", "Z");
                bulkCopy.WriteToServer(ConvertToDataTable(activities.ToList()));
            }

        }

        public static List<Activity> GetActivities(string exportedUserId, DateTime from, DateTime to)
        {
            var activities = new List<Activity>(150000);
            string connectionString = GetConnectionString();
            string commandStr = string.Format(
                "select * from dbo.Activities where UserId = '{0}' and SaveTime > '{1:yyyy-MM-dd HH:mm:ss.fff}' and SaveTime < '{2:yyyy-MM-dd HH:mm:ss.fff}'",
                exportedUserId, @from, @to);


            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(commandStr, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            activities.Add(new Activity
                            {
                                UserId = reader["UserId"].ToString(),
                                SaveTime = DateTime.Parse(reader["SaveTime"].ToString()),
                                X = float.Parse(reader["X"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                                Y = float.Parse(reader["Y"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                                Z = float.Parse(reader["Z"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                            });
                        }
                        catch (Exception) { /* Not reached. */}
                    }
                }
            }
            return activities;
        }

        #region Helpers
        private static DataTable ConvertToDataTable<T>(IEnumerable<T> data)
        {
            var properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        private static string GetConnectionString()
        {
            return System.Configuration
                .ConfigurationManager.ConnectionStrings["DefaultConnection"]
                .ConnectionString;
        }
        #endregion
    }
}
