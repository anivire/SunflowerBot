﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sunflower.Context;

namespace Sunflower.Migrations
{
    [DbContext(typeof(SunflowerUsersContext))]
    partial class SunflowerUsersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0-preview.7.20365.15");

            modelBuilder.Entity("Sunflower.Models.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DailyCooldown")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("MemberId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MemberSunCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MemberUsername")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UserProfiles");
                });
#pragma warning restore 612, 618
        }
    }
}
