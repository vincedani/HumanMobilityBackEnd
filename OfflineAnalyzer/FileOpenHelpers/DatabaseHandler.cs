using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using DataAnalyzer.Entities;
using HumanMobility.Models;

namespace OfflineAnalyzer.FileOpenHelpers
{
    class DatabaseHandler
    {
        public static List<RowType> GetData(string path, out string userName)
        {
            var rows = new List<RowType>();
            string connectionString = GetConnectionString(path);
           
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                userName = GetUserName(connection);

                if (userName == null)
                    return null;

                ProcessRows(rows, connection);
            }
            return rows;
        }

        private static string GetUserName(SQLiteConnection connection)
        {
            var userNames = new List<string>();
            using (var command = new SQLiteCommand("select * from  APPLICATION_USER", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        userNames.Add(reader["USER_NAME"].ToString());
                    }
                }
            }

            if (userNames.Count == 1)
                return userNames.First();

            Console.WriteLine("*** Corrupted database! There are more than one users in it. ***");
            return null;
        }

        private static void ProcessRows(ICollection<RowType> rows, SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand("select * from ACTIVITY_LEVEL", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            rows.Add(new RowType
                            {
                                DateTime = DateTime.ParseExact(reader["_id"].ToString(), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                                X = float.Parse(reader["X"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                                Y = float.Parse(reader["Y"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                                Z = float.Parse(reader["Z"].ToString(), CultureInfo.InvariantCulture.NumberFormat)

                            });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(string.Concat("An exception has thrown: ", e.Message));
                        }
                    }
                }
            }
        }

        private static void ProcessLocations(ICollection<LocationViewModel> rows, SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand("select * from LOCATION", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            rows.Add(new LocationViewModel
                            {
                                SaveTime = DateTime.ParseExact(reader["_id"].ToString(), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                                DetectionTime = DateTime.ParseExact(reader["DETECTION_TIME"].ToString(), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture),
                                Longitude = double.Parse(reader["LONGITUDE"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                                Latitude =  double.Parse(reader["LATITUDE"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                                Error = int.Parse(reader["ERROR"].ToString()) == 1,
                                Accuary = float.Parse(reader["ACCURACY"].ToString(), CultureInfo.InvariantCulture.NumberFormat),
                            });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(string.Concat("An exception has thrown: ", e.Message));
                        }
                    }
                }
            }
        }
        private static string GetConnectionString(string path)
        {
            string connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = path,
                Version = 3
            }.ConnectionString;

            return connectionString;
        }

        public static List<LocationViewModel> GetLocations(string path)
        {
            var rows = new List<LocationViewModel>();
            string connectionString = GetConnectionString(path);

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                ProcessLocations(rows, connection);
            }
            return rows;
        }

    }
}
