using DynamicLinkLibrary.Interfaces;
using DynamicLinkLibrary.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLinkLibrary
{
    public static class StaticHelpers
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static bool IsNullOrEmpty(this JToken token)
        {
            return (token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null);
        }

        public static IpDetails GetLocation(string ipAddress, string apiKey)
        {
            try
            {
                string url = "http://api.ipstack.com/" + ipAddress + $"?access_key={apiKey}";
                var request = WebRequest.Create(url);
                using WebResponse wrs = request.GetResponse();
                using Stream stream = wrs.GetResponseStream();
                using StreamReader reader = new(stream);
                string json = reader.ReadToEnd();
                var obj = JObject.Parse(json);
                if (obj.SelectToken("type").IsNullOrEmpty())
                    throw new IPServiceNotAvailableException($"Couldn't find location for the IP Address -{ipAddress}-");

                string city = (string)obj["city"];
                string country = (string)obj["country_name"];
                string continent = (string)obj["continent_name"];
                string lat = (string)obj["latitude"];
                string lng = (string)obj["longitude"];

                var returnObject = new IpDetails()
                {
                    City = city,
                    Country = country,
                    Continent = continent,
                    Latitude = lat,
                    Longitude = lng
                };

                return returnObject;
            }
            catch (IPServiceNotAvailableException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new IPServiceNotAvailableException(ex.Message);
            }
        }
    }
}
