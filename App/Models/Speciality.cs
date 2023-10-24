using System.Collections.Generic;

namespace App.Models;

public class Speciality {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<Doctor> Doctors { get; set; }
}