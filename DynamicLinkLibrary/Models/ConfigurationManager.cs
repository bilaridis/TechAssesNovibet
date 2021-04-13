using DynamicLinkLibrary.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLinkLibrary.Models
{
    public class ConfigurationManager : IConfigManager
    {
        public string IpStackApiKey
        {
            get
            {
                return this._configuration["DynamicLinkLibrary:IpStack:ApiKey"];
            }
        }

        private readonly IConfiguration _configuration;
        public ConfigurationManager(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
    }
}
