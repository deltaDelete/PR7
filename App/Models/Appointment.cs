using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class Appointment {
    public int AppointmentId { get; set; }

    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }

    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}