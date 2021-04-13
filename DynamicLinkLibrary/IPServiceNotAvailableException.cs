using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicLinkLibrary
{
    [Serializable]
    public class IPServiceNotAvailableException : Exception
    {
        public IPServiceNotAvailableException()
        {

        }

        public IPServiceNotAvailableException(string exceptionMessage)
            : base(String.Format("IP Service Not Available Exception Message: {0}", exceptionMessage))
        {

        }

    }
}
