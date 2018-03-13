using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DataAnalyzer.Entities;

namespace OfflineAnalyzer.FileOpenHelpers
{
    class DistFileHandler
    {
        public static List<ActivityPerMinute> GetData(string path)
        {
            var rows = new List<ActivityPerMinute>();
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

        private static void ProcessRow(ICollection<ActivityPerMinute> rows, string row)
        {
            var columns = row.Split('\t');
            double activity = double.Parse(columns[0].Trim(), CultureInfo.InvariantCulture);
            string date = string.Concat(columns[1].Trim(), " ", columns[2].Trim());
            var dateTime = ParseTimeStamp(date);

            rows.Add(new ActivityPerMinute(dateTime, activity));
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

            if (DateTime.TryParseExact(timeStamp, "yyyy.MM.dd. HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateTime))
                return dateTime;

            if (DateTime.TryParseExact(timeStamp, "yyyy.MM.dd. H:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateTime))
                return dateTime;

            throw new Exception("Couldn't parse the given timestamp format.");
        }
    }
}
