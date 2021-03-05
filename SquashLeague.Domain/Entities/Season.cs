using SquashLeague.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Domain.Entities
{
    public class Season : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
