namespace BancoCore.Models.Response;

public record TransferenciaResultadoResponse(
    string CuentaOrigen,
    string CuentaDestino,
    decimal MontoTransferido,
    decimal SaldoOrigen,
    decimal SaldoDestino
);

public record HistorialTransaccionResponse(
    int Id,
    string Origen,
    string Destino,
    decimal Monto,
    DateTime Fecha,
    string Tipo
);