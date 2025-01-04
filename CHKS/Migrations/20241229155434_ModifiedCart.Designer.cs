﻿// <auto-generated />
using System;
using CHKS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CHKS.Migrations
{
    [DbContext(typeof(mydbContext))]
    [Migration("20241229155434_ModifiedCart")]
    partial class ModifiedCart
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("CHKS.Models.mydb.Car", b =>
                {
                    b.Property<string>("Plate")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Info")
                        .HasColumnType("longtext");

                    b.Property<short?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("Phone")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.HasKey("Plate");

                    b.ToTable("car", t =>
                        {
                            t.HasTrigger("car_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.CarBrand", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Car")
                        .HasColumnType("longtext");

                    b.HasKey("Key");

                    b.ToTable("car_brand", t =>
                        {
                            t.HasTrigger("car_brand_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.Cart", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CartID");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("CartId"));

                    b.Property<string>("Car_Id")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<decimal>("Total")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("CartId");

                    b.HasIndex("Car_Id");

                    b.ToTable("cart", t =>
                        {
                            t.HasTrigger("cart_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.Connector", b =>
                {
                    b.Property<string>("GeneratedKey")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<Guid?>("InventoryId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Note")
                        .HasColumnType("longtext");

                    b.Property<decimal?>("PriceOverwrite")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<decimal>("Qty")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("GeneratedKey");

                    b.HasIndex("CartId");

                    b.HasIndex("InventoryId");

                    b.ToTable("connector", t =>
                        {
                            t.HasTrigger("connector_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.Dailyexpense", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("varchar(255)");

                    b.Property<decimal?>("Expense")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Key");

                    b.ToTable("dailyexpense", t =>
                        {
                            t.HasTrigger("dailyexpense_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.History", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'");

                    b.Property<decimal?>("Baht")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("Bank")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("CashoutDate")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<sbyte?>("Company")
                        .HasColumnType("tinyint");

                    b.Property<decimal?>("Dollar")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("Info")
                        .HasColumnType("longtext");

                    b.Property<string>("Plate")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<decimal?>("Riel")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("Total")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("User")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Plate");

                    b.ToTable("history", t =>
                        {
                            t.HasTrigger("history_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.Historyconnector", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("ID")
                        .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'");

                    b.Property<Guid>("CartId")
                        .HasColumnType("char(36)")
                        .HasColumnName("CartID");

                    b.Property<decimal?>("Export")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<Guid?>("InventoryId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Note")
                        .HasColumnType("longtext");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("char(36)");

                    b.Property<decimal?>("Qty")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("InventoryId");

                    b.ToTable("historyconnector", t =>
                        {
                            t.HasTrigger("historyconnector_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.Inventory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'");

                    b.Property<string>("Barcode")
                        .HasColumnType("longtext");

                    b.Property<decimal?>("Export")
                        .HasPrecision(10, 3)
                        .HasColumnType("decimal(10,3)");

                    b.Property<decimal?>("Import")
                        .HasPrecision(10, 3)
                        .HasColumnType("decimal(10,3)");

                    b.Property<string>("Info")
                        .HasColumnType("longtext");

                    b.Property<short?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValueSql("'0'");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NormalizeName")
                        .HasColumnType("longtext")
                        .HasColumnName("Normalize_Name");

                    b.Property<decimal>("Stock")
                        .HasPrecision(10, 3)
                        .HasColumnType("decimal(10,3)");

                    b.HasKey("Id");

                    b.ToTable("inventory", t =>
                        {
                            t.HasTrigger("inventory_Trigger");
                        });
                });

            modelBuilder.Entity("CHKS.Models.mydb.Cart", b =>
                {
                    b.HasOne("CHKS.Models.mydb.Car", "Car")
                        .WithMany("Carts")
                        .HasForeignKey("Car_Id")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("Car");
                });

            modelBuilder.Entity("CHKS.Models.mydb.Connector", b =>
                {
                    b.HasOne("CHKS.Models.mydb.Cart", "Cart")
                        .WithMany("Connectors")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("CHKS.Models.mydb.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryId");

                    b.Navigation("Cart");

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("CHKS.Models.mydb.History", b =>
                {
                    b.HasOne("CHKS.Models.mydb.Car", "Car")
                        .WithMany("Histories")
                        .HasForeignKey("Plate")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.Navigation("Car");
                });

            modelBuilder.Entity("CHKS.Models.mydb.Historyconnector", b =>
                {
                    b.HasOne("CHKS.Models.mydb.History", "History")
                        .WithMany("Historyconnectors")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.ClientNoAction)
                        .IsRequired();

                    b.HasOne("CHKS.Models.mydb.Inventory", "Inventory")
                        .WithMany()
                        .HasForeignKey("InventoryId");

                    b.Navigation("History");

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("CHKS.Models.mydb.Car", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Histories");
                });

            modelBuilder.Entity("CHKS.Models.mydb.Cart", b =>
                {
                    b.Navigation("Connectors");
                });

            modelBuilder.Entity("CHKS.Models.mydb.History", b =>
                {
                    b.Navigation("Historyconnectors");
                });
#pragma warning restore 612, 618
        }
    }
}
