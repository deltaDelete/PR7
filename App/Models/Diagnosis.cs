using System;

namespace App.Models; 

public class Diagnosis {
    public int Id { get; set; }

    public virtual Patient Patient { get; set; }

    public virtual Doctor Doctor { get; set; }

    public DateTime Date { get; set; }

    public virtual DiagnosisType Type { get; set; }

    public string Comment { get; set; } = string.Empty;
}