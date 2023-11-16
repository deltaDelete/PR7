using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class Drug {
    [Key]
    public int Id { get; set; }

    [MinLength(1, ErrorMessage = "Минимальная длина 1 символ")]
    [Required(ErrorMessage = "Заполните это поле")]
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Medication> Medications { get; set; }
}