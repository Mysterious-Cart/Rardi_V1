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

            builder.Entity<CHKS.Models.mydb.Aspnetuserlogin>().HasKey(table => new {
                table.LoginProvider, table.ProviderKey
            });

            builder.Entity<CHKS.Models.mydb.Aspnetuserrole>().HasKey(table => new {
                table.UserId, table.RoleId
            });

            builder.Entity<CHKS.Models.mydb.Aspnetusertoken>().HasKey(table => new {
                table.UserId, table.LoginProvider, table.Name
            });

            builder.Entity<CHKS.Models.mydb.Aspnetroleclaim>()
              .HasOne(i => i.Aspnetrole)
              .WithMany(i => i.Aspnetroleclaims)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Aspnetuserclaim>()
              .HasOne(i => i.Aspnetuser)
              .WithMany(i => i.Aspnetuserclaims)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Aspnetuserlogin>()
              .HasOne(i => i.Aspnetuser)
              .WithMany(i => i.Aspnetuserlogins)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Aspnetuserrole>()
              .HasOne(i => i.Aspnetrole)
              .WithMany(i => i.Aspnetuserroles)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Aspnetuserrole>()
              .HasOne(i => i.Aspnetuser)
              .WithMany(i => i.Aspnetuserroles)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Aspnetusertoken>()
              .HasOne(i => i.Aspnetuser)
              .WithMany(i => i.Aspnetusertokens)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Cart>()
              .HasOne(i => i.Car)
              .WithMany(i => i.Carts)
              .HasForeignKey(i => i.Plate)
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
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Code)
              .OnDelete(DeleteBehavior.ClientNoAction);

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
              .HasPrincipalKey(i => i.CashoutDate)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Historyconnector>()
              .HasOne(i => i.Inventory)
              .WithMany(i => i.Historyconnectors)
              .HasForeignKey(i => i.Product)
              .HasPrincipalKey(i => i.Code)
              .OnDelete(DeleteBehavior.ClientNoAction);

            builder.Entity<CHKS.Models.mydb.Car>()
              .Property(p => p.IsDeleted)
              .HasDefaultValueSql(@"'0'");

            builder.Entity<CHKS.Models.mydb.Inventory>()
              .Property(p => p.IsDeleted)
              .HasDefaultValueSql(@"'0'");

            builder.Entity<CHKS.Models.mydb.Aspnetuser>()
              .Property(p => p.LockoutEnd)
              .HasColumnType("datetime(6)");

            builder.Entity<CHKS.Models.mydb.Cart>()
              .Property(p => p.Total)
              .HasPrecision(10,2);

            builder.Entity<CHKS.Models.mydb.Cashback>()
              .Property(p => p.Amount)
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

        public DbSet<CHKS.Models.mydb.Efmigrationshistory> Efmigrationshistories { get; set; }

        public DbSet<CHKS.Models.mydb.Aspnetroleclaim> Aspnetroleclaims { get; set; }

        public DbSet<CHKS.Models.mydb.Aspnetrole> Aspnetroles { get; set; }

        public DbSet<CHKS.Models.mydb.Aspnetuserclaim> Aspnetuserclaims { get; set; }

        public DbSet<CHKS.Models.mydb.Aspnetuserlogin> Aspnetuserlogins { get; set; }

        public DbSet<CHKS.Models.mydb.Aspnetuserrole> Aspnetuserroles { get; set; }

        public DbSet<CHKS.Models.mydb.Aspnetuser> Aspnetusers { get; set; }

        public DbSet<CHKS.Models.mydb.Aspnetusertoken> Aspnetusertokens { get; set; }

        public DbSet<CHKS.Models.mydb.Car> Cars { get; set; }

        public DbSet<CHKS.Models.mydb.CarBrand> CarBrands { get; set; }

        public DbSet<CHKS.Models.mydb.Cart> Carts { get; set; }

        public DbSet<CHKS.Models.mydb.Cashback> Cashbacks { get; set; }

        public DbSet<CHKS.Models.mydb.Connector> Connectors { get; set; }

        public DbSet<CHKS.Models.mydb.Dailyexpense> Dailyexpenses { get; set; }

        public DbSet<CHKS.Models.mydb.History> Histories { get; set; }

        public DbSet<CHKS.Models.mydb.Historyconnector> Historyconnectors { get; set; }

        public DbSet<CHKS.Models.mydb.Inventory> Inventories { get; set; }

        public DbSet<CHKS.Models.mydb.InventoryCaroption> InventoryCaroptions { get; set; }

        public DbSet<CHKS.Models.mydb.InventoryOption> InventoryOptions { get; set; }

        public DbSet<CHKS.Models.mydb.InventoryProductgroup> InventoryProductgroups { get; set; }

        public DbSet<CHKS.Models.mydb.Changesrecord> Changesrecords { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
            configurationBuilder.Conventions.Remove(typeof(Microsoft.EntityFrameworkCore.Metadata.Conventions.CascadeDeleteConvention));
        }
    }
}