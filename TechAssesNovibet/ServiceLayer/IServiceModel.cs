using DynamicLinkLibrary.Interfaces;
using System;
using System.Collections.Generic;

namespace TechAssesNovibet.ServiceLayer
{
    public interface IServiceModel
    {
        IIPdetails GetByIP(string ipAddress);
        Guid RegisterBatch(List<string> ipAddresses);
        string GetBatchProcess(Guid processId);
    }
}