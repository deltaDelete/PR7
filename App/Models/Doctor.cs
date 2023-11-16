using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models; 

public class Doctor {
    [Key]
    public int DoctorId { get; set; }
    
    [Required(ErrorMessage = "Заполните это поле")]
    [RegularExpression("^[\\p{L} \\.'\\-]+$", ErrorMessage = "Имя не может содержать цифр или иных спец. символов.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Заполните это поле")]
    [RegularExpression("^(\\d+)$", ErrorMessage = "Неверный формат номера.")]
    public long Phone { get; set; }
    
    [Required(ErrorMessage = "Заполните это поле")]
    [EmailAddress(ErrorMessage = "Неверный формат адреса электронной почты.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Заполните это поле")]
    public virtual Speciality Speciality { get; set; }
    
    public virtual ICollection<Appointment> Appointments { get; set; }
    public virtual ICollection<Diagnosis> Diagnoses { get; set; }
}