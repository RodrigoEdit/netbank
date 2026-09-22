namespace BancoCore.Models.Request;
public record OperacionCuentaRequest(
    string NumeroCuenta,
    decimal Monto
);
public record TransferenciaRequest(
    string CuentaOrigen,
    string CuentaDestino,
    decimal Monto
);