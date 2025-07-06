using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CHKS.Models.mydb;
using CHKS.Models;
namespace CHKS.Data
{
    public partial class Rardi_Context : DbContext
    {
        public Rardi_Context()
        {
        }

        public Rardi_Context(DbContextOptions<Rardi_Context> options) : base(options)
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

      builder.Entity<Employee>()
        .HasMany(i => i.Group)
        .WithMany(i => i.Employee);

      builder.Entity<Order_Model>()
          .HasOne(i => i.Product)
          .WithMany(i => i.Orders)
          .HasForeignKey(i => i.ProductId)
          .HasPrincipalKey(i => i.Id);

      builder.Entity<Order_Model>()
          .Property(i => i.TotalPrice)
          .HasComputedColumnSql(@"
            [Amount] * (
                SELECT p.[Import] 
                FROM [Inventory] p 
                WHERE p.[Id] = [ProductId]
            )");

      builder.Entity<Order_Model>().Property(i => i.OrderDate).HasConversion<DateOnly>();
      builder.Entity<Order_Model>().Property(i => i.OrderReceivedDate).HasConversion<DateOnly>();

      builder.Entity<StockLogs>()
          .HasOne(i => i.Employee)
          .WithMany(i => i.StockLogs)
          .HasForeignKey(i => i.EmployeeId)
          .HasPrincipalKey(i => i.Id);
      builder.Entity<StockLogs>()
          .HasOne(i => i.Product)
          .WithMany(i => i.StockLogs)
          .HasForeignKey(i => i.ProductId);
      builder.Entity<StockLogs>().Property(i => i.Date).IsRowVersion().HasConversion<DateTime>();

      builder.Entity<Customer>()
          .HasOne(i => i.Vehicle)
          .WithMany(i => i.Customer)
          .HasForeignKey(i => i.Vehicle_Id)
          .HasPrincipalKey(i => i.Key);
      builder.Entity<Product_Model>()
          .OwnsMany<ProductProfiles>(i => i.ProductProfiles, b =>
          {
              b.WithOwner().HasForeignKey("ProductId");
              b.Property<Guid>("Id").ValueGeneratedOnAdd();
              b.HasKey("Id");
              b.ToTable("Product_Profiles");
          });
    }

        public DbSet<StockLogs> StockLogs { get; set; }
        public DbSet<Customer> Customer { get; set; }

        public DbSet<Vehicle_Model> Vehicle { get; set; }

        public DbSet<Cart_Model> Carts { get; set; }

        public DbSet<CartItem_Model> Connectors { get; set; }

        public DbSet<Dailyexpense> Dailyexpenses { get; set; }

        public DbSet<History> Histories { get; set; }

        public DbSet<Historyconnector> Historyconnectors { get; set; }

        public DbSet<Product_Model> Inventory { get; set; }

        public DbSet<Order_Model> Order { get; set; }
        public DbSet<Employee> Employee {get; set;}
        public DbSet<Groups> Groups {get; set;}

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
      {
        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        configurationBuilder.Conventions.Remove(typeof(Microsoft.EntityFrameworkCore.Metadata.Conventions.CascadeDeleteConvention));
      }
    }
}