using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class DiagnosisType {
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
}