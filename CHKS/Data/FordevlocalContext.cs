using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHKS.Models.fordevlocal;

namespace CHKS.Data
{
    public partial class fordevlocalContext : DbContext
    {
        public fordevlocalContext()
        {
        }

        public fordevlocalContext(DbContextOptions<fordevlocalContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CHKS.Models.fordevlocal.Cart>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Carts)
              .HasForeignKey(i => i.Plate)
              .HasPrincipalKey(i => i.Plate);

            builder.Entity<CHKS.Models.fordevlocal.Connector>()
              .HasOne(i => i.Cart)
              .WithMany(i => i.Connectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.CartId);

            builder.Entity<CHKS.Models.fordevlocal.Connector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.Connectors)
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<CHKS.Models.fordevlocal.History>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Histories)
              .HasForeignKey(i => i.Plate)
              .HasPrincipalKey(i => i.Plate);

            builder.Entity<CHKS.Models.fordevlocal.Historyconnector>()
              .HasOne(i => i.History)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.CashoutDate);

            builder.Entity<CHKS.Models.fordevlocal.Historyconnector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<CHKS.Models.fordevlocal.Cart>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Connector>()
              .Property(p => p.Qty)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Connector>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Connector>()
              .Property(p => p.Discount)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Connector>()
              .Property(p => p.PriceOverwrite)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Dailyexpense>()
              .Property(p => p.Expense)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.History>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.History>()
              .Property(p => p.Bank)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.History>()
              .Property(p => p.Dollar)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.History>()
              .Property(p => p.Baht)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.History>()
              .Property(p => p.Riel)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Historyconnector>()
              .Property(p => p.Qty)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Historyconnector>()
              .Property(p => p.Export)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordevlocal.Inventory>()
              .Property(p => p.Stock)
              .HasPrecision(10,3);

            builder.Entity<CHKS.Models.fordevlocal.Inventory>()
              .Property(p => p.Import)
              .HasPrecision(10,3);

            builder.Entity<CHKS.Models.fordevlocal.Inventory>()
              .Property(p => p.Export)
              .HasPrecision(10,3);
            this.OnModelBuilding(builder);
        }

        public DbSet<CHKS.Models.fordevlocal.Car> Cars { get; set; }

        public DbSet<CHKS.Models.fordevlocal.Cart> Carts { get; set; }

        public DbSet<CHKS.Models.fordevlocal.Connector> Connectors { get; set; }

        public DbSet<CHKS.Models.fordevlocal.Dailyexpense> Dailyexpenses { get; set; }

        public DbSet<CHKS.Models.fordevlocal.History> Histories { get; set; }

        public DbSet<CHKS.Models.fordevlocal.Historyconnector> Historyconnectors { get; set; }

        public DbSet<CHKS.Models.fordevlocal.Inventory> Inventories { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}