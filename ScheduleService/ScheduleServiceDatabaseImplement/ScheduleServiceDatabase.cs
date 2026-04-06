using Microsoft.EntityFrameworkCore;
using ScheduleServiceDatabaseImplement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceDatabaseImplement
{
    public class ScheduleServiceDatabase : DbContext
    {
        public ScheduleServiceDatabase(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<DutyPerson> DutyPersons { get; set; }

        public virtual DbSet<DutySchedule> DutySchedules { get; set; }

        public virtual DbSet<Group> Groups { get; set; }

        public virtual DbSet<LessonTime> LessonTimes { get; set; }

        public virtual DbSet<ScheduleItem> ScheduleItems { get; set; }

        public virtual DbSet<Teacher> Teachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DutyPerson>(entity =>
            {
                entity.ToTable("duty_persons");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).HasMaxLength(150);
                entity.Property(e => e.Phone).HasMaxLength(30);
                entity.Property(e => e.Email).HasMaxLength(150);
            });

            modelBuilder.Entity<DutySchedule>(entity =>
            {
                entity.ToTable("duty_schedules");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Place).HasMaxLength(200);
                entity.Property(e => e.Comment).HasMaxLength(1000);

                entity.HasOne(e => e.DutyPerson)
                    .WithMany(e => e.DutySchedules)
                    .HasForeignKey(e => e.DutyPersonId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LessonTime)
                    .WithMany(e => e.DutySchedules)
                    .HasForeignKey(e => e.LessonTimeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("groups");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.CoreSystemId).IsRequired();
                entity.Property(e => e.GroupName).IsRequired().HasMaxLength(100);

                entity.HasIndex(e => e.CoreSystemId).IsUnique();
                entity.HasIndex(e => e.GroupName);
            });

            modelBuilder.Entity<LessonTime>(entity =>
            {
                entity.ToTable("lesson_times");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.PairNumber).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);

                entity.HasIndex(e => e.PairNumber).IsUnique();
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("teachers");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.CoreSystemId).IsRequired();
                entity.Property(e => e.TeacherName).IsRequired().HasMaxLength(200);

                entity.HasIndex(e => e.CoreSystemId).IsUnique();
                entity.HasIndex(e => e.TeacherName);
            });

            modelBuilder.Entity<ScheduleItem>(entity =>
            {
                entity.ToTable("schedule_items");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ClassroomNumber).HasMaxLength(50);
                entity.Property(e => e.GroupName).HasMaxLength(100);
                entity.Property(e => e.TeacherName).HasMaxLength(200);
                entity.Property(e => e.Comment).HasMaxLength(1000);

                entity.HasOne(e => e.Group)
                    .WithMany(e => e.ScheduleItems)
                    .HasForeignKey(e => e.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Teacher)
                    .WithMany(e => e.ScheduleItems)
                    .HasForeignKey(e => e.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.LessonTime)
                    .WithMany(e => e.ScheduleItems)
                    .HasForeignKey(e => e.LessonTimeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Date);
            });
        }
    }
}
