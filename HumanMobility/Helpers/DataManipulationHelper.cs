using System;
using System.Collections.Generic;
using HumanMobility.Models;
using System.Device.Location;
using System.Linq;

namespace HumanMobility.Helpers
{
    public static class DataManipulationHelper
    {
        public static List<Location> FillErrorsWithLastData(List<Location> data)
        {
            var lastKnownLocation = data.FirstOrDefault();
            for (int index = 1; index < data.Count; index++)
            {
                var location = data[index];
                int distanceBetweenTimes =
                    Convert.ToInt32((location.SaveTime - lastKnownLocation.SaveTime).TotalMinutes);

                if (distanceBetweenTimes < 0)
                    break;

                if (distanceBetweenTimes > 1)
                {
                    for (int i = 1; i < distanceBetweenTimes; i++)
                    {
                        var savetime = lastKnownLocation.SaveTime.AddMinutes(i);
                        data.Add(new Location
                        {
                            Latitude = lastKnownLocation.Latitude,
                            Longitude = lastKnownLocation.Longitude,
                            Accuary = lastKnownLocation.Accuary,
                            UserId = lastKnownLocation.UserId,
                            Error = lastKnownLocation.Error,
                            DetectionTime = lastKnownLocation.DetectionTime,
                            SaveTime = savetime
                        });
                    }
                }
                lastKnownLocation = location;
            }
            return data.OrderBy(m => m.SaveTime).ToList();
        }

        public static List<Location> FillErrorsWithLinearInterpolation(List<Location> data)
        {
            var lastKnownLocation = data.FirstOrDefault();
            for (int index = 1; index < data.Count; index++)
            {
                var location = data[index];
                int distanceBetweenTimes =
                    Convert.ToInt32((location.SaveTime - lastKnownLocation.SaveTime).TotalMinutes);

                if (distanceBetweenTimes < 0)
                    break;

                if (distanceBetweenTimes > 1)
                {
                    double dLongitude = Math.Abs((location.Longitude - lastKnownLocation.Longitude) / distanceBetweenTimes);
                    double dLatitude = Math.Abs((location.Latitude - lastKnownLocation.Latitude) / distanceBetweenTimes);

                    double longitude = lastKnownLocation.Longitude < location.Longitude
                        ? lastKnownLocation.Longitude
                        : location.Longitude;

                    double latitude = lastKnownLocation.Latitude < location.Latitude
                        ? lastKnownLocation.Latitude
                        : location.Latitude;

                    for (int i = 1; i < distanceBetweenTimes; i++)
                    {
                        data.Add(new Location
                        {
                            Accuary = lastKnownLocation.Accuary,
                            DetectionTime = lastKnownLocation.DetectionTime,
                            Error = lastKnownLocation.Error,
                            Latitude = latitude + i * dLatitude,
                            Longitude = longitude + i * dLongitude,
                            SaveTime = lastKnownLocation.SaveTime.AddMinutes(i),
                            UserId = lastKnownLocation.UserId
                        });
                    }
                }
                lastKnownLocation = location;
            }
            return data.OrderBy(m => m.SaveTime).ToList();
        }

        public static bool IsTwoLocationSame(Location first, Location second)
        {
            return  first.Latitude.Equals(second.Latitude) &&
                         first.Longitude.Equals(second.Longitude);
        }

        public static int DistanceBetweenLocations(Location first, Location second)
        {
            var firstCoord = new GeoCoordinate(first.Latitude, first.Longitude);
            var secondCoord = new GeoCoordinate(second.Latitude, second.Longitude);

            return (int)firstCoord.GetDistanceTo(secondCoord);
        }
    }
}
