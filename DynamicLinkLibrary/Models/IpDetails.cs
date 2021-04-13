using DynamicLinkLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLinkLibrary.Models
{
    public class IpDetails : IIPdetails
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public IpDetails()
        {

        }
        public IpDetails(IIPdetails c)
        {
            City = c.City;
            Country = c.Country;
            Continent = c.Continent;
            Latitude = c.Latitude;
            Longitude = c.Longitude;
        }   
    }       
}           
            