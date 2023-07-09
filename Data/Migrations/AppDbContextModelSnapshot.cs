﻿// <auto-generated />
using DurgerKing.Data;
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

            modelBuilder.Entity("DurgerKing.Entity.User", b =>
                {
                    b.Property<int>("TelegramUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TelegramUserId"));

                    b.Property<string>("Fullname")
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)");

                    b.Property<string>("Language")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(25)
                        .HasColumnType("character varying(25)");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("TelegramUserId");

                    b.ToTable("User");
                });
#pragma warning restore 612, 618
        }
    }
}