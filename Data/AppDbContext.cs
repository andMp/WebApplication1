using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<RozkStr> Streams => Set<RozkStr>();
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<RozkStr>()
                .HasIndex(x => x.StreamerId);
            b.Entity<RozkStr>()
                .HasIndex(x => x.Pochatok);
        }
    }
}
