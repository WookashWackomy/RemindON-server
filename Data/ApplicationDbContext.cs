using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RemindONServer.Enums;
using RemindONServer.Models;

namespace DotNetCoreSqlDb.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<RemindONServer.Models.RemindONDevice> RemindONDevice { get; set; }
        public DbSet<RemindONServer.Models.User> User { get; set; }
        public DbSet<RemindONServer.Models.Prescription> Prescription { get; set; }
        public DbSet<RemindONServer.Models.Check> Check { get; set; }
        public DbSet<DotNetCoreSqlDb.Models.Todo> Todo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Prescription>()
                .Property(e => e.DayTimes)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<DateTime>>(v));

            modelBuilder
                .Entity<Prescription>()
                .Property(e => e.WeekDays)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<DayOfWeek>>(v));

            modelBuilder
                .Entity<Check>()
                .Property(e => e.Flag)
                .HasConversion(
                    v => v.ToString(),
                    v => (Flag)Enum.Parse(typeof(Flag), v));

            base.OnModelCreating(modelBuilder);
        }
    }
}
