using Microsoft.EntityFrameworkCore;
using ManagementContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementContract.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Consultant> Consultants { get; set; }

        public DbSet<Contract> Contracts { get; set; }

        public DbSet<ConsultantContract> ConsultantContract { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConsultantContract>()
                .HasKey(cc => new { cc.ContractId, cc.ConsultantId });

            modelBuilder.Entity<ConsultantContract>()
                .HasOne(cc => cc.Contract)
                .WithMany(c => c.Consultants)
                .HasForeignKey(cc => cc.ContractId);

            modelBuilder.Entity<ConsultantContract>()
                .HasOne(cc => cc.Consultant)
                .WithMany(c => c.Contracts)
                .HasForeignKey(cc => cc.ConsultantId);
        }
    }

}
