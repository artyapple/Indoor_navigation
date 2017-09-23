using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using MassApp.Models;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace MassApp.Services
{
    public class TouristicApiService
    {
        public async Task<EventInfo> GetEvents()
        {
            HttpClient c = new HttpClient();
            Uri uri = new Uri($"https://hht.infomaxnet.de/imxeventmanager2/service?user=ws.Hochbahn&password=ttGbVXzuh&method=FindEventIds&eGeoSearchDistance=0.2&eGeoSearchLong=9.993682&eGeoSearchLat=53.551086&eImagesAvailable=true");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            string resStr = string.Empty;
            using (HttpResponseMessage httpRes = await c.SendAsync(requestMessage))
            {
                resStr = await httpRes.Content.ReadAsStringAsync();
            }

            MatchCollection collection = Regex.Matches(resStr, "[0-9]{3,}");
            foreach(Match match in collection)
            {
               
            }

            return new EventInfo("","");
        }
    }
}
