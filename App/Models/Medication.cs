using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class Medication {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public decimal Dosage { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public string Frequency { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Заполните это поле")]
    public virtual Drug Drug { get; set; }

    [Required(ErrorMessage = "Заполните это поле")]
    public virtual Patient Patient { get; set; }
}