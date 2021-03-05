using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Domain.Common
{
    public class AuditableEntity
    {
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
