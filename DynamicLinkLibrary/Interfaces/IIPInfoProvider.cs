using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLinkLibrary.Interfaces
{
    public interface IIPInfoProvider
    {
        IIPdetails GetIPdetails(string ipAddress);
    }
}
