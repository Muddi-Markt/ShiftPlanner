﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Muddi.ShiftPlanner.Server.Database.Contexts;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    [DbContext(typeof(ShiftPlannerContext))]
    partial class ShiftPlannerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.Shift", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("EmployeeKeycloakId")
                        .HasColumnType("uuid")
                        .HasColumnName("employee_keycloak_id");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end");

                    b.Property<Guid?>("ShiftContainerId")
                        .HasColumnType("uuid")
                        .HasColumnName("shift_container_id");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start");

                    b.Property<Guid>("TypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("type_id");

                    b.HasKey("Id")
                        .HasName("pk_shifts");

                    b.HasIndex("ShiftContainerId")
                        .HasDatabaseName("ix_shifts_shift_container_id");

                    b.HasIndex("TypeId")
                        .HasDatabaseName("ix_shifts_type_id");

                    b.ToTable("shifts", (string)null);
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftContainer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTime>("End")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end");

                    b.Property<Guid>("FrameworkId")
                        .HasColumnType("uuid")
                        .HasColumnName("framework_id");

                    b.Property<Guid?>("ShiftLocationId")
                        .HasColumnType("uuid")
                        .HasColumnName("shift_location_id");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start");

                    b.Property<int>("TotalShifts")
                        .HasColumnType("integer")
                        .HasColumnName("total_shifts");

                    b.HasKey("Id")
                        .HasName("pk_containers");

                    b.HasIndex("FrameworkId")
                        .HasDatabaseName("ix_containers_framework_id");

                    b.HasIndex("ShiftLocationId")
                        .HasDatabaseName("ix_containers_shift_location_id");

                    b.ToTable("containers", (string)null);
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftFramework", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int>("SecondsPerShift")
                        .HasColumnType("integer")
                        .HasColumnName("seconds_per_shift");

                    b.HasKey("Id")
                        .HasName("pk_shift_frameworks");

                    b.ToTable("shift_frameworks", (string)null);
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftFrameworkTypeCount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("Count")
                        .HasColumnType("integer")
                        .HasColumnName("count");

                    b.Property<Guid>("ShiftFrameworkId")
                        .HasColumnType("uuid")
                        .HasColumnName("shift_framework_id");

                    b.Property<Guid>("ShiftTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("shift_type_id");

                    b.HasKey("Id")
                        .HasName("pk_shift_framework_type_count");

                    b.HasIndex("ShiftFrameworkId")
                        .HasDatabaseName("ix_shift_framework_type_count_shift_framework_id");

                    b.HasIndex("ShiftTypeId")
                        .HasDatabaseName("ix_shift_framework_type_count_shift_type_id");

                    b.ToTable("shift_framework_type_count", (string)null);
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftLocation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid>("TypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("type_id");

                    b.HasKey("Id")
                        .HasName("pk_shift_locations");

                    b.HasIndex("TypeId")
                        .HasDatabaseName("ix_shift_locations_type_id");

                    b.ToTable("shift_locations", (string)null);
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftLocationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_shift_location_types");

                    b.ToTable("shift_location_types", (string)null);
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_shift_types");

                    b.ToTable("shift_types", (string)null);
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.Shift", b =>
                {
                    b.HasOne("Muddi.ShiftPlanner.Server.Database.Entities.ShiftContainer", null)
                        .WithMany("Shifts")
                        .HasForeignKey("ShiftContainerId")
                        .HasConstraintName("fk_shifts_containers_shift_container_id");

                    b.HasOne("Muddi.ShiftPlanner.Server.Database.Entities.ShiftType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shifts_shift_types_type_id");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftContainer", b =>
                {
                    b.HasOne("Muddi.ShiftPlanner.Server.Database.Entities.ShiftFramework", "Framework")
                        .WithMany()
                        .HasForeignKey("FrameworkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_containers_shift_frameworks_framework_id");

                    b.HasOne("Muddi.ShiftPlanner.Server.Database.Entities.ShiftLocation", null)
                        .WithMany("Containers")
                        .HasForeignKey("ShiftLocationId")
                        .HasConstraintName("fk_containers_shift_locations_shift_location_id");

                    b.Navigation("Framework");
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftFrameworkTypeCount", b =>
                {
                    b.HasOne("Muddi.ShiftPlanner.Server.Database.Entities.ShiftFramework", "ShiftFramework")
                        .WithMany("ShiftTypeCounts")
                        .HasForeignKey("ShiftFrameworkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shift_framework_type_count_shift_frameworks_shift_framework_id");

                    b.HasOne("Muddi.ShiftPlanner.Server.Database.Entities.ShiftType", "ShiftType")
                        .WithMany()
                        .HasForeignKey("ShiftTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shift_framework_type_count_shift_types_shift_type_id");

                    b.Navigation("ShiftFramework");

                    b.Navigation("ShiftType");
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftLocation", b =>
                {
                    b.HasOne("Muddi.ShiftPlanner.Server.Database.Entities.ShiftLocationType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_shift_locations_shift_location_types_type_id");

                    b.Navigation("Type");
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftContainer", b =>
                {
                    b.Navigation("Shifts");
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftFramework", b =>
                {
                    b.Navigation("ShiftTypeCounts");
                });

            modelBuilder.Entity("Muddi.ShiftPlanner.Server.Database.Entities.ShiftLocation", b =>
                {
                    b.Navigation("Containers");
                });
#pragma warning restore 612, 618
        }
    }
}