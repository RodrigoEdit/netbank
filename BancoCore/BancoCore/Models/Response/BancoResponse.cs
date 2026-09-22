namespace BancoCore.Models.Response;
public record ConsultaCuentaResponse(
    int Id,
    string NumeroCuenta,
    string Titular,
    decimal Saldo
);
public record DepositoResponse(
    string NumeroCuenta,
    decimal MontoDepositado,
    decimal SaldoActual
);
public record RetiroResponse(
    string NumeroCuenta,
    decimal MontoRetirado,
    decimal SaldoActual
);