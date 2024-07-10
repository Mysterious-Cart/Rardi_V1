using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHKS.Models.fordev;

namespace CHKS.Data
{
    public partial class fordevContext : DbContext
    {
        public fordevContext()
        {
        }

        public fordevContext(DbContextOptions<fordevContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CHKS.Models.fordev.Cart>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Carts)
              .HasForeignKey(i => i.Plate)
              .HasPrincipalKey(i => i.Plate);

            builder.Entity<CHKS.Models.fordev.Connector>()
              .HasOne(i => i.Cart)
              .WithMany(i => i.Connectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.CartId);

            builder.Entity<CHKS.Models.fordev.Connector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.Connectors)
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<CHKS.Models.fordev.History>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Histories)
              .HasForeignKey(i => i.Plate)
              .HasPrincipalKey(i => i.Plate);

            builder.Entity<CHKS.Models.fordev.Historyconnector>()
              .HasOne(i => i.History)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.CashoutDate);

            builder.Entity<CHKS.Models.fordev.Historyconnector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Name);

            builder.Entity<CHKS.Models.fordev.Cart>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.Connector>()
              .Property(p => p.Qty)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.Connector>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.Dailyexpense>()
              .Property(p => p.Expense)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.Expensehistoryconnector>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.History>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.History>()
              .Property(p => p.Bank)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.History>()
              .Property(p => p.Dollar)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.History>()
              .Property(p => p.Baht)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.History>()
              .Property(p => p.Riel)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.Historyconnector>()
              .Property(p => p.Qty)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.Historyconnector>()
              .Property(p => p.Export)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.fordev.Inventory>()
              .Property(p => p.Stock)
              .HasPrecision(10,3);

            builder.Entity<CHKS.Models.fordev.Inventory>()
              .Property(p => p.Import)
              .HasPrecision(10,3);

            builder.Entity<CHKS.Models.fordev.Inventory>()
              .Property(p => p.Export)
              .HasPrecision(10,3);
            this.OnModelBuilding(builder);
        }

        public DbSet<CHKS.Models.fordev.Car> Cars { get; set; }

        public DbSet<CHKS.Models.fordev.Cart> Carts { get; set; }

        public DbSet<CHKS.Models.fordev.Connector> Connectors { get; set; }

        public DbSet<CHKS.Models.fordev.Dailyexpense> Dailyexpenses { get; set; }

        public DbSet<CHKS.Models.fordev.Expensehistoryconnector> Expensehistoryconnectors { get; set; }

        public DbSet<CHKS.Models.fordev.History> Histories { get; set; }

        public DbSet<CHKS.Models.fordev.Historyconnector> Historyconnectors { get; set; }

        public DbSet<CHKS.Models.fordev.Inventory> Inventories { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}