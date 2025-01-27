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
              .HasForeignKey(i => i.Car_Id)
              .HasPrincipalKey(i => i.Plate)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Connector>()
              .HasOne(i => i.Cart)
              .WithMany(i => i.Connectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.CartId)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Connector>()
            .HasOne(i => i.Inventory)
            .WithMany(i => i.Connectors)
            .HasForeignKey(i => i.ProductId)
            .HasPrincipalKey(i => i.Id)
            .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Tags>()
            .HasMany(i => i.Product)
            .WithMany(i => i.Tags);

            builder.Entity<CHKS.Models.mydb.History>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Histories)
              .HasForeignKey(i => i.Plate)
              .HasPrincipalKey(i => i.Plate)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .HasOne(i => i.History)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.CartId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.HistoryConnectors)
              .HasForeignKey(i => i.ProductId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Car>()
              .Property(p => p.IsDeleted)
              .HasDefaultValueSql(@"'0'");

            builder.Entity<CHKS.Models.mydb.History>()
              .Property(p => p.Id)
              .HasDefaultValueSql(@"'00000000-0000-0000-0000-000000000000'").ValueGeneratedOnAdd();

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .Property(p => p.Id)
              .HasDefaultValueSql(@"'00000000-0000-0000-0000-000000000000'").ValueGeneratedOnAdd();

            builder.Entity<CHKS.Models.mydb.Inventory>()
              .Property(p => p.IsDeleted)
              .HasDefaultValueSql(@"'0'");

            builder.Entity<CHKS.Models.mydb.Inventory>()
              .Property(p => p.Id)
              .HasDefaultValueSql(@"'00000000-0000-0000-0000-000000000000'").ValueGeneratedOnAdd();

            builder.Entity<CHKS.Models.mydb.Cart>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.Connector>()
              .Property(p => p.Qty)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.Connector>()
              .Property(p => p.PriceOverwrite)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.Dailyexpense>()
              .Property(p => p.Expense)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.History>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.History>()
              .Property(p => p.Bank)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.History>()
              .Property(p => p.Dollar)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.History>()
              .Property(p => p.Baht)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.History>()
              .Property(p => p.Riel)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .Property(p => p.Qty)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .Property(p => p.Export)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.Inventory>()
              .Property(p => p.Stock)
              .HasPrecision(10,3);

            builder.Entity<CHKS.Models.mydb.Inventory>()
              .Property(p => p.Import)
              .HasPrecision(10,3);

            builder.Entity<CHKS.Models.mydb.Inventory>()
              .Property(p => p.Export)
              .HasPrecision(10,3);
            this.OnModelBuilding(builder);
        }

        public DbSet<CHKS.Models.mydb.Car> Cars { get; set; }

        public DbSet<CHKS.Models.mydb.CarBrand> CarBrands { get; set; }

        public DbSet<CHKS.Models.mydb.Cart> Carts { get; set; }

        public DbSet<CHKS.Models.mydb.Connector> Connectors { get; set; }

        public DbSet<CHKS.Models.mydb.Dailyexpense> Dailyexpenses { get; set; }

        public DbSet<CHKS.Models.mydb.History> Histories { get; set; }

        public DbSet<CHKS.Models.mydb.Historyconnector> Historyconnectors { get; set; }

        public DbSet<CHKS.Models.mydb.Inventory> Inventories { get; set; }

        public DbSet<Records> Records {get; set;}

        public DbSet<Daily> Daily {get; set;}

        public DbSet<Tags> Tags {get; set;}

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
            configurationBuilder.Conventions.Remove(typeof(Microsoft.EntityFrameworkCore.Metadata.Conventions.CascadeDeleteConvention));
        }
    }
}