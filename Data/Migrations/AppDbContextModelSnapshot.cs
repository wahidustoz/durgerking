﻿// <auto-generated />
using System;
using DurgerKing.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace durgerking.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DurgerKing.Entity.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(125)
                        .HasColumnType("character varying(125)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Food"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Snack"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Drink"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Salad"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Set"
                        });
                });

            modelBuilder.Entity("DurgerKing.Entity.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Longitute")
                        .HasColumnType("numeric");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("DurgerKing.Entity.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<float>("DiscountPercentage")
                        .HasColumnType("real");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("DurgerKing.Entity.ProductMedium", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<byte[]>("Data")
                        .HasColumnType("bytea");

                    b.Property<string>("Extension")
                        .HasColumnType("text");

                    b.Property<string>("Filename")
                        .HasColumnType("text");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductMedia");
                });

            modelBuilder.Entity("DurgerKing.Entity.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6142));

                    b.Property<string>("Fullname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Language")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<DateTime>("ModifiedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6376));

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Username")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ProductProduct", b =>
                {
                    b.Property<Guid>("ItemsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.HasKey("ItemsId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductProduct");
                });

            modelBuilder.Entity("DurgerKing.Entity.Location", b =>
                {
                    b.HasOne("DurgerKing.Entity.User", "User")
                        .WithMany("Locations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DurgerKing.Entity.Product", b =>
                {
                    b.HasOne("DurgerKing.Entity.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("DurgerKing.Entity.ProductMedium", b =>
                {
                    b.HasOne("DurgerKing.Entity.Product", "Product")
                        .WithMany("Media")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ProductProduct", b =>
                {
                    b.HasOne("DurgerKing.Entity.Product", null)
                        .WithMany()
                        .HasForeignKey("ItemsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DurgerKing.Entity.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DurgerKing.Entity.Product", b =>
                {
                    b.Navigation("Media");
                });

            modelBuilder.Entity("DurgerKing.Entity.User", b =>
                {
                    b.Navigation("Locations");
                });
#pragma warning restore 612, 618
        }
    }
}
