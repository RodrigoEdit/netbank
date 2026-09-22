namespace BancoCore.Models;

public class Cuenta
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public string Titular { get; set; } = string.Empty;
    public string Moneda { get; set; } = "PEN";
}