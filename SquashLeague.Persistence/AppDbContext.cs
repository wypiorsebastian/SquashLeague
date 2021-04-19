using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SquashLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SquashLeague.Application.Models;

namespace SquashLeague.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public AppDbContext(DbContextOptions options)
            : base(options)
        {

        }
    }
}
