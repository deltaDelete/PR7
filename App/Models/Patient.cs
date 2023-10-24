using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class Patient
{
    [Key] public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public virtual ICollection<Medication> Medications { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
    public virtual ICollection<Diagnosis> Diagnoses { get; set; }
}