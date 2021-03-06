using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemindONServer.Domain.Models;

namespace RemindONServer.Domain.Persistence.Contexts
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RemindONDevice> RemindONDevices { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Check> Checks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Prescription>()
                .Property(e => e.DayTimes)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<TimeSpan>>(v));

            modelBuilder
                .Entity<Prescription>()
                .Property(e => e.WeekDays)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<DayOfWeek>>(v));

            base.OnModelCreating(modelBuilder);
        }
    }
}
