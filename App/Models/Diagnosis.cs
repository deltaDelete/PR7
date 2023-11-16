using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class Diagnosis {
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Заполните это поле")]
    public virtual Patient Patient { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public virtual Doctor Doctor { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public DateTimeOffset Date { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public virtual DiagnosisType Type { get; set; }

    public string Comment { get; set; } = string.Empty;
}