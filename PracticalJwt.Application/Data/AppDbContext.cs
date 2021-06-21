using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PracticalJwt.Domain.Models;
using System;

namespace PracticalJwt.Application.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder ob)
        {
            var cs = new SqliteConnectionStringBuilder()
            {
                DataSource = "main.db",
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();

            ob.UseSqlite(cs);
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>()
                .HasOne(m => m.RefreshToken)
                .WithOne(s => s.User)
                .HasForeignKey<RefreshToken>(f => f.UserId);

            //sqlite uses string dates
            mb.Entity<RefreshToken>().Property(p => p.ExpiresAt)
                .HasConversion<DateTime>();
        }
    }
}
