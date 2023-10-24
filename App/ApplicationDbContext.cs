using System.Data.Common;
using App.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace App;

public class ApplicationDbContext : DbContext {
    private static readonly DbConnectionStringBuilder DbConnectionStringBuilder = new MySqlConnectionStringBuilder() {
        Server = "10.10.1.24",
        Database = "pro1_2",
        UserID = "user_01",
        Password = "user01pro",
    };

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Diagnosis> Diagnoses { get; set; }
    public DbSet<DiagnosisType> DiagnosisTypes { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Drug> Drugs { get; set; }
    public DbSet<Medication> Medications { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Speciality> Specialities { get; set; }

    public ApplicationDbContext() : base() {
        Database.EnsureCreated();
        Database.OpenConnection();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseMySql(
            DbConnectionStringBuilder.ConnectionString,
            ServerVersion.Create(8, 0, 15, ServerType.MySql));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
    }
}