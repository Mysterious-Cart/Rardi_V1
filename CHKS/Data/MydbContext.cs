using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHKS.Models.mydb;

namespace CHKS.Data
{
    public partial class mydbContext : DbContext
    {
        public mydbContext()
        {
        }

        public mydbContext(DbContextOptions<mydbContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CHKS.Models.mydb.Cart>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Carts)
              .HasForeignKey(i => i.Plate)
              .HasPrincipalKey(i => i.Plate);

            builder.Entity<CHKS.Models.mydb.Connector>()
              .HasOne(i => i.Cart)
              .WithMany(i => i.Connectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.CartId);

            builder.Entity<CHKS.Models.mydb.Connector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.Connectors)
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<CHKS.Models.mydb.History>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Histories)
              .HasForeignKey(i => i.Plate)
              .HasPrincipalKey(i => i.Plate);

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .HasOne(i => i.History)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.CashoutDate);

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Name);
            this.OnModelBuilding(builder);
        }

        public DbSet<CHKS.Models.mydb.Car> Cars { get; set; }

        public DbSet<CHKS.Models.mydb.Cart> Carts { get; set; }

        public DbSet<CHKS.Models.mydb.Connector> Connectors { get; set; }

        public DbSet<CHKS.Models.mydb.Daily> Dailies { get; set; }

        public DbSet<CHKS.Models.mydb.Dailyexpense> Dailyexpenses { get; set; }

        public DbSet<CHKS.Models.mydb.History> Histories { get; set; }

        public DbSet<CHKS.Models.mydb.Historyconnector> Historyconnectors { get; set; }

        public DbSet<CHKS.Models.mydb.Inventory> Inventories { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}