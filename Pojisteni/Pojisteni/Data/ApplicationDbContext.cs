using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Pojisteni.Models;
using System.Diagnostics.Contracts;
using System.Reflection.Emit;

namespace Pojisteni.Data
{
    public class ApplicationDbContext : IdentityDbContext<AccountIdentity>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Pojisteni.Models.Insured>? Insured { get; set; }
        public DbSet<Pojisteni.Models.Insurance>? Insurance { get; set; }
        public DbSet<Pojisteni.Models.Event>? Event { get; set; }
        public DbSet<Pojisteni.Models.InsuranceInsured>? InsuranceInsured { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Insured>().HasIndex(e => e.Email).IsUnique(true);
        }
    }
}