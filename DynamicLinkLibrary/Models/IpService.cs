using DynamicLinkLibrary.Interfaces;
using System;

namespace DynamicLinkLibrary.Models
{
    public class IpService : IIPInfoProvider
    {
        private IConfigManager _config;
        public IpService(IConfigManager config)
        {
            _config = config;
        }
        public IIPdetails GetIPdetails(string ipAddress)
        {
            try
            {
                return StaticHelpers.GetLocation(ipAddress, _config.IpStackApiKey);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
