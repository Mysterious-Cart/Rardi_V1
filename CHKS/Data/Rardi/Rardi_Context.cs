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

      builder.Entity<CartModel>()
        .HasOne(i => i.Customer)
        .WithMany(i => i.Carts)
        .HasForeignKey(i => i.CustomerId)
        .HasPrincipalKey(i => i.PlateNumber)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<CartItemModel>()
        .HasOne(i => i.Cart)
        .WithMany(i => i.CartContent)
        .HasForeignKey(i => i.CartId)
        .HasPrincipalKey(i => i.CartId)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<CartItemModel>()
        .HasOne(i => i.Inventory)
        .WithMany(i => i.CartItems)
        .HasForeignKey(i => i.ProductId)
        .HasPrincipalKey(i => i.Id)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<Tags>()
        .HasMany(i => i.Product)
        .WithMany(i => i.Tags);

      builder.Entity<TransactionModel>()
        .HasOne(i => i.Customer)
        .WithMany(i => i.Transactions)
        .HasForeignKey(i => i.Plate)
        .HasPrincipalKey(i => i.PlateNumber)
        .OnDelete(DeleteBehavior.ClientNoAction);
      builder.Entity<TransactionModel>()
        .OwnsMany(i => i.Payments, b =>
        {
            b.WithOwner();
            b.ToTable("Transaction_Payments");
        });

      builder.Entity<TransactionItemModel>()
        .HasOne(i => i.Transaction)
        .WithMany(i => i.TransactionItems)
        .HasForeignKey(i => i.TransactionId)
        .HasPrincipalKey(i => i.Id)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<TransactionItemModel>()
        .HasOne(i => i.Product)
        .WithMany(i => i.TransactionItems)
        .HasForeignKey(i => i.ProductId)
        .HasPrincipalKey(i => i.Id)
        .OnDelete(DeleteBehavior.ClientNoAction);

      builder.Entity<EmployeeModel>()
        .HasMany(i => i.Group)
        .WithMany(i => i.Employee);

      builder.Entity<OrderModel>()
          .HasOne(i => i.Product)
          .WithMany(i => i.Orders)
          .HasForeignKey(i => i.ProductId)
          .HasPrincipalKey(i => i.Id);

      builder.Entity<OrderModel>()
          .Property(i => i.TotalPrice)
          .HasComputedColumnSql(@"
            [Amount] * (
                SELECT p.[Import] 
                FROM [Inventory] p 
                WHERE p.[Id] = [ProductId]
            )");

      builder.Entity<OrderModel>()
          .Property(i => i.OrderDate).HasConversion<DateOnly>();
      builder.Entity<OrderModel>()
          .Property(i => i.OrderReceivedDate).HasConversion<DateOnly>();

      builder.Entity<StockLogs>()
          .HasOne(i => i.Employee)
          .WithMany(i => i.StockLogs)
          .HasForeignKey(i => i.EmployeeId)
          .HasPrincipalKey(i => i.Id);
      builder.Entity<StockLogs>()
          .HasOne(i => i.Product)
          .WithMany(i => i.StockLogs)
          .HasForeignKey(i => i.ProductId);
      builder.Entity<StockLogs>()
          .Property(i => i.Date)
          .IsRowVersion()
          .HasConversion<DateTime>();

      builder.Entity<CustomerModel>()
          .HasOne(i => i.Vehicle)
          .WithMany(i => i.Customer)
          .HasForeignKey(i => i.Vehicle_Id)
          .HasPrincipalKey(i => i.Key);
      
      builder.Entity<Product_Model>()
          .OwnsMany(i => i.ProductProfiles, b =>
          {
              b.WithOwner().HasForeignKey("ProductId");
              b.Property<Guid>("Id").ValueGeneratedOnAdd();
              b.HasKey("Id");
              b.ToTable("Product_Profiles");
          });
    }

        public DbSet<StockLogs> StockLogs { get; set; }
        public DbSet<CustomerModel> Customers { get; set; }

        public DbSet<Vehicle_Model> Vehicles { get; set; }

        public DbSet<CartModel> Carts { get; set; }

        public DbSet<CartItemModel> CartContents { get; set; }

        public DbSet<TransactionModel> Transactions { get; set; }

        public DbSet<TransactionItemModel> TransactionItems { get; set; }

        public DbSet<Product_Model> Inventory { get; set; }

        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<EmployeeModel> Employees {get; set;}
        public DbSet<GroupModel> Groups {get; set;}

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
      {
        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        configurationBuilder.Conventions.Remove(typeof(Microsoft.EntityFrameworkCore.Metadata.Conventions.CascadeDeleteConvention));
      }
    }
}