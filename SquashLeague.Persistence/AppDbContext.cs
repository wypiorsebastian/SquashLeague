﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SquashLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Season> Seasons { get; set; }
        public AppDbContext(DbContextOptions options)
            : base(options)
        {

        }
    }
}