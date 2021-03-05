using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Application.CQRS.Seasons.DTOs
{
    public class SeasonCreateDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
