using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class Doctor {
    [Key]
    public int DoctorId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public virtual Speciality Speciality { get; set; }
    
    public virtual ICollection<Appointment> Appointments { get; set; }
    public virtual ICollection<Diagnosis> Diagnoses { get; set; }
}