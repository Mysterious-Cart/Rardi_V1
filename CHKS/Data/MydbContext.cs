using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHKS.Models.mydb;
using CHKS.Entity;

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

      builder.Entity<Cart_Model>()
        .HasOne(i => i.Customer)
        .WithMany(i => i.Carts)
        .HasForeignKey(i => i.Car_Id)
        .HasPrincipalKey(i => i.Plate)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<CartItem_Model>()
        .HasOne(i => i.Cart)
        .WithMany(i => i.CartContent)
        .HasForeignKey(i => i.CartId)
        .HasPrincipalKey(i => i.CartId)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<CartItem_Model>()
      .HasOne(i => i.Inventory)
      .WithMany(i => i.Connectors)
      .HasForeignKey(i => i.ProductId)
      .HasPrincipalKey(i => i.Id)
      .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<Tags>()
      .HasMany(i => i.Product)
      .WithMany(i => i.Tags);

      builder.Entity<History>()
        .HasOne(i => i.Car)
        .WithMany(i => i.Histories)
        .HasForeignKey(i => i.Plate)
        .HasPrincipalKey(i => i.Plate)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<Historyconnector>()
        .HasOne(i => i.History)
        .WithMany(i => i.Historyconnectors)
        .HasForeignKey(i => i.CartId)
        .HasPrincipalKey(i => i.Id)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<Historyconnector>()
        .HasOne(i => i.Inventory)
        .WithMany(i => i.HistoryConnectors)
        .HasForeignKey(i => i.ProductId)
        .HasPrincipalKey(i => i.Id)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<Order>()
          .HasOne(i => i.Product)
          .WithMany(i => i.Orders)
          .HasForeignKey(i => i.ProductId)
          .HasPrincipalKey(i => i.Id);

      builder.Entity<Order>().Property(i => i.OrderDate).HasConversion<DateOnly>();
      builder.Entity<Order>().Property(i => i.OrderReceivedDate).HasConversion<DateOnly>();

      builder.Entity<Customer_Model>()
          .HasOne(i => i.Vehicle)
          .WithMany(i => i.Customer)
          .HasForeignKey(i => i.Vehicle_Id)
          .HasPrincipalKey(i => i.Key);

      builder.Entity<History>()
        .Property(p => p.Id)
        .HasDefaultValueSql(@"'00000000-0000-0000-0000-000000000000'").ValueGeneratedOnAdd();

      builder.Entity<Historyconnector>()
        .Property(p => p.Id)
        .HasDefaultValueSql(@"'00000000-0000-0000-0000-000000000000'").ValueGeneratedOnAdd();

      builder.Entity<Product_Model>()
        .Property(p => p.Id)
        .HasDefaultValueSql(@"'00000000-0000-0000-0000-000000000000'").ValueGeneratedOnAdd();

      builder.Entity<Cart_Model>()
        .Property(p => p.Total)
        .HasPrecision(10, 2);

      builder.Entity<CartItem_Model>()
        .Property(p => p.Qty)
        .HasPrecision(10, 2);

      builder.Entity<CartItem_Model>()
        .Property(p => p.PriceOverwrite)
        .HasPrecision(10, 2);

      builder.Entity<Dailyexpense>()
        .Property(p => p.Expense)
        .HasPrecision(10, 2);

      builder.Entity<History>()
        .Property(p => p.Total)
        .HasPrecision(10, 2);

      builder.Entity<History>()
        .Property(p => p.Bank)
        .HasPrecision(10, 2);

      builder.Entity<History>()
        .Property(p => p.Dollar)
        .HasPrecision(10, 2);

      builder.Entity<History>()
        .Property(p => p.Baht)
        .HasPrecision(10, 2);

      builder.Entity<History>()
        .Property(p => p.Riel)
        .HasPrecision(10, 2);

      builder.Entity<Historyconnector>()
        .Property(p => p.Qty)
        .HasPrecision(10, 2);

      builder.Entity<Historyconnector>()
        .Property(p => p.Export)
        .HasPrecision(10, 2);

      builder.Entity<Product_Model>()
        .Property(p => p.Stock)
        .HasPrecision(10, 3);

      builder.Entity<Product_Model>()
        .Property(p => p.Import)
        .HasPrecision(10, 3);

      builder.Entity<Product_Model>()
        .Property(p => p.Export)
        .HasPrecision(10, 3);
      this.OnModelBuilding(builder);

        }
        public DbSet<Customer_Model> Customer { get; set; }

        public DbSet<Vehicle_Model> Vehicle { get; set; }

        public DbSet<Cart_Model> Carts { get; set; }

        public DbSet<CartItem_Model> Connectors { get; set; }

        public DbSet<Dailyexpense> Dailyexpenses { get; set; }

        public DbSet<History> Histories { get; set; }

        public DbSet<Historyconnector> Historyconnectors { get; set; }

        public DbSet<Product_Model> Inventories { get; set; }

        public DbSet<Order> Order { get; set; }

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