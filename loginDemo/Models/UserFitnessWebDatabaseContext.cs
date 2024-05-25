using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace loginDemo.Models;

public partial class UserFitnessWebDatabaseContext : DbContext
{
    public UserFitnessWebDatabaseContext()
    {
    }

    public UserFitnessWebDatabaseContext(DbContextOptions<UserFitnessWebDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCity> TblCities { get; set; }

    public virtual DbSet<TblTodo> TblTodos { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserRate> UserRates { get; set; }

    public DbSet<TblTodo> TblFavorites { get; set; } // TblTodo tablosu için DbSet özelliği eklendi
    public object FavoriteModel { get; internal set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = WebApplication.CreateBuilder();
        var connectionString = builder.Configuration.GetConnectionString ("MyConnection");
        optionsBuilder.UseSqlServer(connectionString);
    }    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblCity>(entity =>
        {
            entity.ToTable("tbl_city");
entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("city");
        });

        modelBuilder.Entity<TblTodo>(entity =>
        {
            entity.ToTable("tbl_todo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("category");
            entity.Property(e => e.Difficulty)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("difficulty");
            entity.Property(e => e.Instruction)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Instruction");


            entity.Property(e => e.IsDeleted).HasColumnName("isDeleted");
            entity.Property(e => e.Period)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("period");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("userId");
        });
modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.ToTable("userDetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("userId");
        });

        modelBuilder.Entity<UserRate>(entity =>
            {
                entity.ToTable("userRate");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Rate).HasColumnName("rate");
                entity.Property(e => e.TodoId).HasColumnName("todoId");
                entity.Property(e => e.UserId).HasMaxLength(450).HasColumnName("userId");

    entity.Property<string>("Comment")
        .HasColumnType("nvarchar(max)")
        .HasColumnName("comment");
            });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}