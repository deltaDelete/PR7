using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class Patient
{
    [Key] public int Id { get; set; }
    
    [Required(ErrorMessage = "Заполните это поле")]
    [RegularExpression("^[\\p{L} \\.'\\-]+$", ErrorMessage = "Имя не может содержать цифр или иных спец. символов.")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Заполните это поле")]
    public string Address { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Заполните это поле")]
    [RegularExpression("^(\\d+)$", ErrorMessage = "Неверный формат номера.")]
    public long Phone { get; set; }
    
    [Required(ErrorMessage = "Заполните это поле")]
    [EmailAddress(ErrorMessage = "Неверный формат адреса электронной почты.")]
    public string Email { get; set; } = string.Empty;
    
    public virtual ICollection<Medication> Medications { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; }
    public virtual ICollection<Diagnosis> Diagnoses { get; set; }
}