using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using HumanMobility.Models;

namespace OfflineAnalyzer.Sync
{
    class SyncLocations
    {
        public static void Syncronize(List<LocationViewModel> rows)
        {
            string url = "http://humanmobility.azurewebsites.net/api/Locations";
            var serializer = new JavaScriptSerializer();

            for (int i = 0; i < rows.Count; i+=1000)
            {
                var tmp = rows.Skip(i).Take(1000).ToList();

                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers[HttpRequestHeader.Authorization] =
                        "Bearer TODO:GET TOKEN";

                    string result = client.UploadString(url, "POST", serializer.Serialize(tmp));

                }
            }
        }
    }
}
