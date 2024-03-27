namespace CitroenAPI.Models.DbContextModels;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


public class CitroenDbContext : DbContext
{

    public CitroenDbContext(DbContextOptions<CitroenDbContext> options)
    : base(options)
    {
    }

    public virtual DbSet<Logs> Logs { get; set; }
    public virtual DbSet<StatusLeads> StatusLeads { get; set; }
    public virtual DbSet<ApiCalls> ApiCalls { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Data Source=SQL5110.site4now.net;Initial Catalog=db_a056d0_citroenapidb;User Id=db_a056d0_citroenapidb_admin;Password=Citroen_api2024");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Logs>(entity =>
        {
            entity.HasKey(e => e.GitId);
            entity.Property(e => e.GitId)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.DispatchDate)
                .IsRequired();

            entity.Property(e => e.CreatedDate)
                .IsRequired();

        });

        modelBuilder.Entity<StatusLeads>(entitiy =>
        {
            entitiy.HasKey(e => e.GitId);
            entitiy.Property(e => e.GitId)
                .IsRequired()
                .HasMaxLength(255);

            entitiy.Property(e => e.Status)
                .IsRequired();

            entitiy.Property(e => e.Status)
                .IsRequired();
        });

        modelBuilder.Entity<ApiCalls>(entitiy =>
        {
            entitiy.HasKey(e => e.Id);
            entitiy.Property(e => e.Id)
                .IsRequired();

            entitiy.Property(e => e.CallDateTime)
                .IsRequired();

            entitiy.Property(e => e.Status)
                .IsRequired();
        });

    }

}