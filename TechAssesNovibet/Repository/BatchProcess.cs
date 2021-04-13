using System;
using System.Collections.Generic;

#nullable disable

namespace TechAssesNovibet.Repository
{
    public partial class BatchProcess
    {
        public BatchProcess()
        {
            BatchId = Guid.NewGuid();
            BatchAddresses = new HashSet<BatchAddress>();
            IsCompleted = false;
        }

        public Guid BatchId { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? Finished { get; set; }
        public string Status { get; set; }
        public string ExceptionMessage { get; set; }

        public virtual ICollection<BatchAddress> BatchAddresses { get; set; }
    }
}
