using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class Appointment {
    public int AppointmentId { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public virtual Patient Patient { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public virtual Doctor Doctor { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public DateTimeOffset Date { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public TimeSpan Time { get; set; }

    public string Type { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}