using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace HumanMobility.Models.Services
{
    public interface IService
    {
        IEnumerable<string> GetUserNames();
        string GetUserId(string userName);

        IEnumerable<Location> GetLocations(string userId, DateTime from, DateTime? too);
        IEnumerable<Location> GetLast24HoursLocation(string userId);
        void StoreLocations(IEnumerable<Location> locations);

        UserCredential GetUserCredential(string userId);
        IEnumerable<BugReport> GetBugReports(IPrincipal user);
        bool StoreUserInfo(UserCredential credential);

        IEnumerable<Place> GetPlaces(string userId);
        bool StorePlace(Place place);
        bool DeletePlace(Place place);


        void LogExort(string adminId, string userId, DateTime from, DateTime to);
        void WriteBugReport(string userId, string message);

        IEnumerable<Activity> GetActivities(string userId, DateTime from, DateTime? too);
    }
}
