using System.Data.Common;
using App.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace App;

/// <inheritdoc cref="DbContext"/>
public class ApplicationDbContext : DbContext {
    private static readonly string ConnectionString =
        "server=10.10.1.24;user=user_01;password=user01pro;database=pro1_2";

    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<Diagnosis> Diagnoses { get; set; } = null!;
    public DbSet<DiagnosisType> DiagnosisTypes { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<Drug> Drugs { get; set; } = null!;
    public DbSet<Medication> Medications { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<Speciality> Specialities { get; set; } = null!;

    /// <inheritdoc cref="DbContext"/>
    public ApplicationDbContext() {
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseMySql(
            ConnectionString,
            ServerVersion.Create(8, 0, 15, ServerType.MySql)
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
    }
}