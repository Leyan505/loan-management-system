﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PrestamosCreciendo.Data;

#nullable disable

namespace PrestamosCreciendo.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240725033306_AddBillsTable")]
    partial class AddBillsTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PrestamosCreciendo.Models.AgentHasClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Id_agent")
                        .HasColumnType("integer");

                    b.Property<int>("Id_client")
                        .HasColumnType("integer");

                    b.Property<int>("Id_wallet")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AgentHasClient");
                });

            modelBuilder.Entity("PrestamosCreciendo.Models.AgentHasSupervisor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Base")
                        .HasColumnType("real");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("IdAgent")
                        .HasColumnType("integer");

                    b.Property<int>("IdSupervisor")
                        .HasColumnType("integer");

                    b.Property<int>("IdWallet")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AgentSupervisor");
                });

            modelBuilder.Entity("PrestamosCreciendo.Models.Bills", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Id_agent")
                        .HasColumnType("integer");

                    b.Property<int>("Id_wallet")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Bills");
                });

            modelBuilder.Entity("PrestamosCreciendo.Models.CloseDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Base_before")
                        .HasColumnType("real");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<float>("From_number")
                        .HasColumnType("real");

                    b.Property<int>("Id_agent")
                        .HasColumnType("integer");

                    b.Property<int>("Id_supervisor")
                        .HasColumnType("integer");

                    b.Property<float>("Total")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("CloseDay");
                });

            modelBuilder.Entity("PrestamosCreciendo.Models.Credit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Amount_neto")
                        .HasColumnType("real");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Id_agent")
                        .HasColumnType("integer");

                    b.Property<int>("Id_user")
                        .HasColumnType("integer");

                    b.Property<int>("Order_list")
                        .HasColumnType("integer");

                    b.Property<int>("Payment_number")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<float>("Utility")
                        .HasColumnType("real");

                    b.HasKey("Id");

                    b.ToTable("Credit");
                });

            modelBuilder.Entity("PrestamosCreciendo.Models.Summary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Id_agent")
                        .HasColumnType("integer");

                    b.Property<int>("Id_credit")
                        .HasColumnType("integer");

                    b.Property<int>("Number_index")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Summary");
                });

            modelBuilder.Entity("PrestamosCreciendo.Models.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<string>("Level")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nit")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<int?>("Phone")
                        .HasColumnType("integer");

                    b.Property<string>("Province")
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("lat")
                        .HasColumnType("text");

                    b.Property<string>("lng")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PrestamosCreciendo.Models.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Country")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created_at")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Wallets");
                });
#pragma warning restore 612, 618
        }
    }
}
