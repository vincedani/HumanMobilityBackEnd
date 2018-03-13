using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using HumanMobility.Models;
using HumanMobility.Models.Services;
using Microsoft.AspNet.Identity;
// ReSharper disable PossibleMultipleEnumeration

namespace HumanMobility.Controllers
{
    [Authorize]
    [RoutePrefix("api")]
    public class RestController : ApiController
    {
        #region Members

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private IService _service;

        #endregion

        #region Constructors

        public RestController(IService service)
        {
            _service = service;
        }

        public RestController()
        {
            _service = new Service();
        }
        #endregion

        #region User info

        // GET api/UserInfo
        [Route("UserInfo")]
        public IHttpActionResult GetUserInfo()
        {
            var userInfo = _service.GetUserCredential(User.Identity.GetUserId());

            if (userInfo == null)
                return Ok();

            return Ok(new UserInfoViewModel()
            {
                DeviceInfo = userInfo.DeviceInfo,
                HasAccelerometer = userInfo.HasAccelerometer,
                HasTemperatureSensor = userInfo.HasTemperatureSensor,
                Version = userInfo.Version
            });
        }

        // POST api/UserInfo
        [Route("UserInfo")]
        public IHttpActionResult UpdateUserInfo(UserInfoViewModel userInfoViewModel)
        {
            if (userInfoViewModel == null)
                return BadRequest("Something went wrong. Empty request.");

            var credential = new UserCredential
            {
                UserId = User.Identity.GetUserId(),
                DeviceInfo = userInfoViewModel.DeviceInfo,
                HasAccelerometer = userInfoViewModel.HasAccelerometer,
                HasTemperatureSensor = userInfoViewModel.HasTemperatureSensor,
                Version = userInfoViewModel.Version
            };

            return _service.StoreUserInfo(credential) ? (IHttpActionResult)Ok() : BadRequest();
        }

        #endregion

        #region Location and Favourite places
        [HttpPost]
        [Route("Place")]
        public IHttpActionResult Place(PlaceViewModel placeViewModel)
        {
            if (placeViewModel == null || !ModelState.IsValid)
                return BadRequest("Something went wrong. Empty request.");

            var place = new Place
            {
                UserId = User.Identity.GetUserId(),
                Latitude = placeViewModel.Latitude,
                Longitude = placeViewModel.Longitude,
                Radius = placeViewModel.Radius,
                Title = placeViewModel.Title,
                Type = placeViewModel.Type
            };

            return _service.StorePlace(place) ? (IHttpActionResult)Ok() : BadRequest();
        }

        [Route("Place")]
        public IHttpActionResult GetPlaces()
        {
            var places = _service.GetPlaces(User.Identity.GetUserId());

            var placeViewModels = places.Select(place => new PlaceViewModel
                {
                    Longitude = place.Longitude,
                    Latitude = place.Latitude,
                    Title = place.Title,
                    Radius = place.Radius,
                    Type = place.Type
                })
                .ToList();

            return Ok(placeViewModels);
        }

        [Route("Place")]
        public IHttpActionResult DeletePlace(PlaceViewModel placeViewModel)
        {
            if (placeViewModel == null || !ModelState.IsValid)
                return BadRequest("Something went wrong. Empty request.");

            var place = new Place
            {
                UserId = User.Identity.GetUserId(),
                Latitude = placeViewModel.Latitude,
                Longitude = placeViewModel.Longitude,
                Radius = placeViewModel.Radius,
                Title = placeViewModel.Title,
                Type = placeViewModel.Type
            };
            
            return _service.DeletePlace(place) ? (IHttpActionResult)Ok() : BadRequest();
        }

        [HttpPost]
        [Route("Locations")]
        public IHttpActionResult Locations(IEnumerable<LocationViewModel> locationViewModels)
        {
            if (locationViewModels == null || !locationViewModels.Any() || !ModelState.IsValid)
                return BadRequest("Something went wrong. Empty request.");

            string userId = User.Identity.GetUserId();
            var locations = locationViewModels.Select(viewModel => new Location
            {
                UserId = userId,
                Accuary = viewModel.Accuary,
                DetectionTime = viewModel.DetectionTime,
                Error = viewModel.Error,
                Latitude = viewModel.Latitude,
                Longitude = viewModel.Longitude,
                SaveTime = viewModel.SaveTime
            });

            for (int i = 0; i < locations.Count(); i += 100)
                _service.StoreLocations(locations.Skip(i).Take(100));

            return Ok();
        }
        #endregion

        #region Bug reporting
        [HttpPost]
        [Route("Bug")]
        public IHttpActionResult Bug(List<BugReportViewModel> reports)
        {
            if (!ModelState.IsValid || reports == null)
                return BadRequest("Something went wrong. Empty request.");

            string userId = User.Identity.GetUserId();
            foreach (var report in reports)
                _service.WriteBugReport(userId, report.Message);

            return Ok();
        }
        #endregion

        #region Accelerometer 

        [HttpPost]
        [Route("Actigraphy")]
        public async Task<IHttpActionResult> Actigraphy()
        {
            // https://stackify.com/understanding-asp-net-performance-for-reading-incoming-data/
            string data = await Request.Content.ReadAsStringAsync();
            var viewModels = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ActivityViewModel>>(data);

            if(viewModels == null || !viewModels.Any())
                return BadRequest("Something went wrong. Empty request.");

            string userId = User.Identity.GetUserId();

            try
            {
                BulkCommands.StoreActivities(
                    viewModels.Select(m =>
                        {
                            m.UserId = userId;
                            return m;
                        })
                        .ToList());
            }
            catch (Exception e)
            {
                _service.WriteBugReport(userId, e.Message);
            }
            return Ok();
        }

        #endregion
    }
}
