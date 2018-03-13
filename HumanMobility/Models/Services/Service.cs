using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HumanMobility.Migrations;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace HumanMobility.Models.Services
{
    public class Service : IService, IDisposable
    {
        #region Members
        private readonly ApplicationDbContext _context;
        #endregion

        #region Constructors
        public Service(ApplicationDbContext context)
        {
            _context = context;
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.ValidateOnSaveEnabled = false;
        }

        public Service()
        {
            _context = new ApplicationDbContext();
            _context.Configuration.AutoDetectChangesEnabled = false;
            _context.Configuration.ValidateOnSaveEnabled = false;
        }
        #endregion

        #region Interface implementation

        #region User info
        public IEnumerable<string> GetUserNames()
        {
            var userRole = _context.Roles.SingleOrDefault(r => r.Name.Equals(Configuration.UserRole));
            var users = _context.Users.Where(u => u.Roles.All(r => r.RoleId == userRole.Id));

            return users.Select(u => u.UserName).AsEnumerable();
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public string GetUserId(string userName)
        {
            return _context.Users.SingleOrDefault(u => u.UserName == userName).Id;
        }

        public UserCredential GetUserCredential(string userId)
        {
            var userCredential = _context.UserCredentials.Where(
                m => m.UserId == userId);

            return userCredential.SingleOrDefault();
        }

        public bool StoreUserInfo(UserCredential credential)
        {
            try
            {
                var cred = GetUserCredential(credential.UserId);

                if (cred == null)
                {
                    _context.UserCredentials.Add(credential);
                }
                else
                {
                    cred.DeviceInfo = credential.DeviceInfo;
                    cred.HasAccelerometer = credential.HasAccelerometer;
                    cred.HasTemperatureSensor = credential.HasTemperatureSensor;
                    cred.Version = credential.Version;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                if (exception.InnerException?.InnerException?.Message != null)
                    message = string.Concat(message, exception.InnerException.InnerException.Message);

                WriteBugReport(credential.UserId, message);
                return false;
            }
        }
        #endregion

        #region Locations
        public IEnumerable<Location> GetLocations(string userId, DateTime from, DateTime? too = null)
        {
            _context.Database.CommandTimeout = 600;
            var to = too ?? DateTime.Today;
            
            var locations = _context.Locations.Where(
                m =>
                m.UserId == userId &&
                m.SaveTime >= from &&
                m.SaveTime <= to
            );

            return locations.OrderBy(m => m.SaveTime);
        }

        public IEnumerable<Location> GetLast24HoursLocation(string userId)
        {
            _context.Database.CommandTimeout = 600;
            var yesterday = DateTime.Now.AddDays(-1);
            var locations = _context.Locations.Where(
                m => 
                m.UserId == userId &&
                m.SaveTime >= yesterday
            );

            return locations.OrderBy(m => m.SaveTime);
        }

        public void StoreLocations(IEnumerable<Location> locations)
        {
            locations = locations.DistinctBy(m => m.SaveTime);
            try
            {
                _context.Locations.AddRange(locations);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // ignored
            }
        }
        #endregion

        #region Activity
        public IEnumerable<Activity> GetActivities(string userId, DateTime @from, DateTime? too = null)
        {
            var to = too ?? DateTime.Now;

            var activities = _context.Activities.Where(
                m =>
                m.UserId == userId &&
                m.SaveTime >= from &&
                m.SaveTime <= to
            );

            return activities.OrderBy(m => m.SaveTime);
        }
        #endregion

        #region Favourite places
        public IEnumerable<Place> GetPlaces(string userId)
        {
            var places = _context.Places.Where(m => m.UserId == userId);
            return places.OrderBy(m => m.Title);
        }

        public bool StorePlace(Place place)
        {
            try
            {
                _context.Places.Add(place);
                _context.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                if (exception.InnerException?.InnerException?.Message != null)
                    message = string.Concat(message, exception.InnerException.InnerException.Message);

                WriteBugReport(place.UserId, message);
                return false;
            }
        }

        public bool DeletePlace(Place place)
        {
            try
            {
                var dbPlace = _context.Places.First(
                    m =>
                        m.UserId == place.UserId &&
                        m.Latitude.Equals(place.Latitude) &&
                        m.Longitude.Equals(place.Longitude) &&
                        m.Radius.Equals(place.Radius) &&
                        m.Title == place.Title &&
                        m.Type == place.Type
                );

                _context.Places.Remove(dbPlace);
                _context.SaveChanges();
                return true;
            }
            catch (Exception exception)
            {
                string message = exception.Message;

                if (exception.InnerException?.InnerException?.Message != null)
                    message = string.Concat(message, exception.InnerException.InnerException.Message);

                WriteBugReport(place.UserId, message);
                return false;
            }
            
        }
        #endregion

        #region Logging
        public void LogExort(string adminId, string userId, DateTime from, DateTime to)
        {
            _context.Logs.Add(new Log
            {
                AdminId = adminId,
                UserId = userId,
                Time = DateTime.Now,
                From = from,
                To = to
            });
            _context.SaveChanges();
        }

        public void WriteBugReport(string userId, string message)
        {
            if(message.Length > 300)
                message = message.Substring(0, 300);

            RejectChanges();
            _context.BugReports.Add(new BugReport
            {
                UserId = userId,
                Date = DateTime.Now,
                Message = message
            });

            _context.SaveChanges();
        }

        public IEnumerable<BugReport> GetBugReports(IPrincipal user)
        {
            string userId = user.Identity.GetUserId();
            var reports = user.IsInRole(Configuration.AdminRole) ? 
                _context.BugReports.ToList() : 
                _context.BugReports.Where(m => m.UserId == userId).ToList();

            return reports.OrderByDescending(m => m.Date);
        }
        #endregion

        public void Dispose()
        {
            _context.Dispose();
        }
        #endregion

        #region Helpers
        private void RejectChanges()
        {
            foreach (var entry in _context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                }
            }
        }
        #endregion
    }
}
