namespace BancoCore.Models.Request;
public record ConsultaCuentaRequest(
    int CuentaId
);
public record DepositoRequest(
    int CuentaId,
    decimal Monto
);
public record RetiroRequest(
    int CuentaId,
    decimal Monto
);
