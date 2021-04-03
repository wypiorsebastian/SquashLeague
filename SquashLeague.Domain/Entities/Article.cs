using SquashLeague.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Domain.Entities
{
    public class Article : Entity
    {
        public string Header { get; set; }
        public string Summary { get; set; }
        public string MainContent { get; set; }
        public ICollection<Image> Images { get; set; }

    }
}
