using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DataAnalyzer.Entities;

namespace OfflineAnalyzer.FileOpenHelpers
{
    class TextFileHandler
    {
        public static List<RowType> GetData(string path)
        {
            var rows = new List<RowType>();
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new StreamReader(fileStream))
                {
                    string row;
                    while ((row = reader.ReadLine()) != null)
                    {
                        try
                        {
                            ProcessRow(rows, row);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(string.Concat("An exception has thrown: ", e.Message, ", Row: ", row));
                        }
                    }
                }
            }
            return rows;
        }

        private static void ProcessRow(ICollection<RowType> rows, string row)
        {
            var columns = row.Split(';');
            string date = columns[0].Trim();
            string x = columns[1].Trim();
            string y = columns[2].Trim();
            string z = columns[3].Trim();

            var dateTime = ParseTimeStamp(date);

            rows.Add(new RowType
            {
                DateTime = dateTime,
                X = float.Parse(x, CultureInfo.InvariantCulture.NumberFormat),
                Y = float.Parse(y, CultureInfo.InvariantCulture.NumberFormat),
                Z = float.Parse(z, CultureInfo.InvariantCulture.NumberFormat)
            });
        }

        private static DateTime ParseTimeStamp(string timeStamp)
        {
            DateTime dateTime;

            if (DateTime.TryParseExact(timeStamp, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateTime))
                return dateTime;

            if (DateTime.TryParseExact(timeStamp, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateTime))
                return dateTime;

            throw new Exception("Couldn't parse the given timestamp format.");
        }

    }
}
