namespace Postes.Lifts.IO.Infrastructure.Models;

public class CarezzaResortModel
{
    public string LiftsStatus { get; set; } = string.Empty;

    public string SlopesStatus { get; set; } = string.Empty;

    public bool Configured => !string.IsNullOrEmpty(LiftsStatus) && !string.IsNullOrEmpty(SlopesStatus);
}
