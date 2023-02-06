﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(TradeMatchingEngineContext))]
    partial class TradeMatchingEngineContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TradeMatchingEngine.Order", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExpireTime")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsFillAndKill")
                        .HasColumnType("bit");

                    b.Property<long?>("OrderParentId")
                        .HasColumnType("bigint");

                    b.Property<int>("OrderState")
                        .HasColumnType("int");

                    b.Property<int?>("OriginalAmount")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Side")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("TradeMatchingEngine.Trade", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<long>("BuyOrderId")
                        .HasColumnType("bigint");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<long>("SellOrderId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("BuyOrderId");

                    b.HasIndex("SellOrderId");

                    b.ToTable("Trades");
                });

            modelBuilder.Entity("TradeMatchingEngine.Trade", b =>
                {
                    b.HasOne("TradeMatchingEngine.Order", null)
                        .WithMany()
                        .HasForeignKey("BuyOrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TradeMatchingEngine.Order", null)
                        .WithMany()
                        .HasForeignKey("SellOrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
