﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using StudentsDashboard.Contexts;

#nullable disable

namespace TinyUrl.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230421163036_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.3.23174.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TinyUrl.Models.UserClick", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ClickedAt")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("clicked_at");

                    b.Property<string>("OriginalUrl")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("original_url");

                    b.Property<string>("TinyUrl")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tiny_url");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.ToTable("UserClicks");
                });
#pragma warning restore 612, 618
        }
    }
}