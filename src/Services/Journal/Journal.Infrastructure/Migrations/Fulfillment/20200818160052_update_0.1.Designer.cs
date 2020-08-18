﻿// <auto-generated />
using System;
using Journal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Journal.Infrastructure.Migrations.Fulfillment
{
    [DbContext(typeof(FulfillmentContext))]
    [Migration("20200818160052_update_0.1")]
    partial class update_01
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Journal.Domain.AggregatesModel.JobAggregate.Fulfillment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ArticlesCount")
                        .HasColumnType("int");

                    b.Property<string>("EventId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExtractionFulfillmentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InsertedCount")
                        .HasColumnType("int");

                    b.Property<bool>("IsPending")
                        .HasColumnType("bit");

                    b.Property<bool>("IsProcessed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JobCompletedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("JobStartedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("JournalType")
                        .HasColumnType("int");

                    b.Property<double>("ProcessingTimeInMilliseconds")
                        .HasColumnType("float");

                    b.Property<DateTime>("QueryFromDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("QueryToDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TotalProcessedCount")
                        .HasColumnType("int");

                    b.Property<int>("UpdatedCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Fulfillments");
                });
#pragma warning restore 612, 618
        }
    }
}
