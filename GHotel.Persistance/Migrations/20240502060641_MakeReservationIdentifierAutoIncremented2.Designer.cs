﻿// <auto-generated />
using System;
using GHotel.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GHotel.Persistance.Migrations
{
    [DbContext(typeof(GHotelDBContext))]
    [Migration("20240502060641_MakeReservationIdentifierAutoIncremented2")]
    partial class MakeReservationIdentifierAutoIncremented2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("GHotel.Domain.Entities.Entity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("EntitiyStatus")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Entity");
                });

            modelBuilder.Entity("GHotel.Domain.Entities.MyImage", b =>
                {
                    b.HasBaseType("GHotel.Domain.Entities.Entity");

                    b.Property<string>("RoomId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasIndex("RoomId");

                    b.ToTable("Images", (string)null);
                });

            modelBuilder.Entity("GHotel.Domain.Entities.MyUser", b =>
                {
                    b.HasBaseType("GHotel.Domain.Entities.Entity");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasFilter("[Email] IS NOT NULL");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Payment", b =>
                {
                    b.HasBaseType("GHotel.Domain.Entities.Entity");

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<int>("Method")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.ToTable("Payments", (string)null);
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Reservation", b =>
                {
                    b.HasBaseType("GHotel.Domain.Entities.Entity");

                    b.Property<DateTime>("CheckInDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CheckOutDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Identifier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Identifier"), 1L, 1);

                    b.Property<int>("NumberOfGuests")
                        .HasColumnType("int");

                    b.Property<string>("PaymentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoomId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasIndex("PaymentId")
                        .IsUnique()
                        .HasFilter("[PaymentId] IS NOT NULL");

                    b.HasIndex("RoomId");

                    b.HasIndex("UserId");

                    b.ToTable("Reservations", (string)null);
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Room", b =>
                {
                    b.HasBaseType("GHotel.Domain.Entities.Entity");

                    b.Property<int>("Capacity")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<decimal>("PricePerNight")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("PricePerNightCurrency")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.ToTable("Rooms", (string)null);
                });

            modelBuilder.Entity("GHotel.Domain.Entities.MyImage", b =>
                {
                    b.HasOne("GHotel.Domain.Entities.Entity", null)
                        .WithOne()
                        .HasForeignKey("GHotel.Domain.Entities.MyImage", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("GHotel.Domain.Entities.Room", null)
                        .WithMany("Images")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("GHotel.Domain.Entities.MyUser", b =>
                {
                    b.HasOne("GHotel.Domain.Entities.Entity", null)
                        .WithOne()
                        .HasForeignKey("GHotel.Domain.Entities.MyUser", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Payment", b =>
                {
                    b.HasOne("GHotel.Domain.Entities.Entity", null)
                        .WithOne()
                        .HasForeignKey("GHotel.Domain.Entities.Payment", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Reservation", b =>
                {
                    b.HasOne("GHotel.Domain.Entities.Entity", null)
                        .WithOne()
                        .HasForeignKey("GHotel.Domain.Entities.Reservation", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("GHotel.Domain.Entities.Payment", "Payment")
                        .WithOne("Reservation")
                        .HasForeignKey("GHotel.Domain.Entities.Reservation", "PaymentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GHotel.Domain.Entities.Room", "Room")
                        .WithMany("Reservations")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GHotel.Domain.Entities.MyUser", "User")
                        .WithMany("Reservations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Payment");

                    b.Navigation("Room");

                    b.Navigation("User");
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Room", b =>
                {
                    b.HasOne("GHotel.Domain.Entities.Entity", null)
                        .WithOne()
                        .HasForeignKey("GHotel.Domain.Entities.Room", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GHotel.Domain.Entities.MyUser", b =>
                {
                    b.Navigation("Reservations");
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Payment", b =>
                {
                    b.Navigation("Reservation")
                        .IsRequired();
                });

            modelBuilder.Entity("GHotel.Domain.Entities.Room", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
