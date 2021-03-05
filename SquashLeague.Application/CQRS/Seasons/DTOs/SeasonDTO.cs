using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Application.CQRS.Seasons.DTOs
{
    public class SeasonDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
