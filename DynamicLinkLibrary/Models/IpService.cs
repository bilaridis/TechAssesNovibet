using DynamicLinkLibrary.Interfaces;
using System;

namespace DynamicLinkLibrary.Models
{
    public class IpService : IIPInfoProvider
    { 
        public IIPdetails GetIPdetails(string ipAddress)
        {
            try
            {
                return StaticHelpers.GetLocation(ipAddress);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
