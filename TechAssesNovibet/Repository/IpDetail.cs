using DynamicLinkLibrary.Interfaces;
using DynamicLinkLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TechAssesNovibet.Repository
{
    public partial class IpDetail : IpDetails
    {
        public IpDetail()
        {
        }
        public IpDetail(IIPdetails baseModel, string ipAddress) : base(baseModel)
        {
            IpAddress = ipAddress;
        }

        [Key]
        public string IpAddress { get; set; }

        [NotMapped]
        public Guid BatchProcessId { get; set; }

    }
}
