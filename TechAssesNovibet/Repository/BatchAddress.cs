using System;
using System.Collections.Generic;

#nullable disable

namespace TechAssesNovibet.Repository
{
    public partial class BatchAddress
    {
        public BatchAddress()
        {
            IsCompleted = false;
        }
        public Guid BatchAddressId { get; set; }
        public string IpAddress { get; set; }
        public Guid? FBatchId { get; set; }
        public bool? IsCompleted { get; set; }

        public virtual BatchProcess FBatch { get; set; }
    }
}
