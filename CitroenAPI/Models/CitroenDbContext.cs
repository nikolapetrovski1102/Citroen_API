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
    => optionsBuilder.UseSqlServer("Data Source=(local);Initial Catalog=CitroenApi;Integrated security=true;TrustServerCertificate=True;Trusted_Connection=True;");

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