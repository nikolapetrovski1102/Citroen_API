namespace CitroenAPI.Models;
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Data Source=SQL5110.site4now.net;Initial Catalog=db_a056d0_citroenapidb;User Id=db_a056d0_citroenapidb_admin;Password=Citroen_api2024");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Logs>(entity =>
        {
            entity.HasKey(e => e.GitId);
            entity.Property(e => e.GitId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.DispatchDate)
                .IsRequired();
        });
    }

}