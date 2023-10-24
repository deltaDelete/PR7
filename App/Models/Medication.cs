using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class Medication {
    [Key]
    public int Id { get; set; }

    public decimal Dosage { get; set; }

    public string Frequency { get; set; } = string.Empty;
    
    public virtual Drug Drug { get; set; }

    public virtual Patient Patient { get; set; }
}