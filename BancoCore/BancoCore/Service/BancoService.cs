using BancoCore.Common;
using BancoCore.Data;
using BancoCore.Models;
using BancoCore.Models.Request;
using BancoCore.Models.Response;
using BancoCore.Services;
using Microsoft.EntityFrameworkCore;

namespace BancoCore.Service;

public class BancoService : IBancoService
{
    private readonly BancoDbContext _context;

    public BancoService(BancoDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<ConsultaCuentaResponse>> ObtenerCuentaPorId(ConsultaCuentaRequest request)
    {
        var cuenta = await _context.Cuentas.FindAsync(request.CuentaId);
        if (cuenta == null)
            return ApiResponse<ConsultaCuentaResponse>.Error("Cuenta no encontrada.");

        var data = new ConsultaCuentaResponse(cuenta.Id, cuenta.NumeroCuenta, cuenta.Titular, cuenta.Saldo);
        return ApiResponse<ConsultaCuentaResponse>.Ok(data, "Cuenta obtenida con éxito.");
    }

    public async Task<ApiResponse<DepositoResponse>> Depositar(DepositoRequest request)
    {
        if (request.Monto <= 0)
            return ApiResponse<DepositoResponse>.Error("El monto a depositar debe ser mayor a 0.");

        var cuenta = await _context.Cuentas.FindAsync(request.CuentaId);
        if (cuenta == null)
            return ApiResponse<DepositoResponse>.Error("Cuenta no encontrada.");

        cuenta.Saldo += request.Monto;

        var movimiento = new Transaccion
        {
            NumeroCuentaOrigen = "VENTANILLA",
            NumeroCuentaDestino = cuenta.NumeroCuenta,
            Monto = request.Monto,
            Fecha = DateTime.UtcNow,
            Tipo = "Deposito"
        };

        _context.Transaccion.Add(movimiento);
        await _context.SaveChangesAsync();

        var data = new DepositoResponse(cuenta.NumeroCuenta, request.Monto, cuenta.Saldo);
        return ApiResponse<DepositoResponse>.Ok(data, "Depósito procesado con éxito.");
    }

    public async Task<ApiResponse<RetiroResponse>> Retirar(RetiroRequest request)
    {
        if (request.Monto <= 0)
            return ApiResponse<RetiroResponse>.Error("El monto a retirar debe ser mayor a 0.");

        var cuenta = await _context.Cuentas.FindAsync(request.CuentaId);
        if (cuenta == null)
            return ApiResponse<RetiroResponse>.Error("Cuenta no encontrada.");

        if (cuenta.Saldo < request.Monto)
            return ApiResponse<RetiroResponse>.Error("Saldo insuficiente para realizar el retiro.");

        cuenta.Saldo -= request.Monto;

        var movimiento = new Transaccion
        {
            NumeroCuentaOrigen = cuenta.NumeroCuenta,
            NumeroCuentaDestino = "CAJERO",
            Monto = request.Monto,
            Fecha = DateTime.UtcNow,
            Tipo = "Retiro"
        };

        _context.Transaccion.Add(movimiento);
        await _context.SaveChangesAsync();

        var data = new RetiroResponse(cuenta.NumeroCuenta, request.Monto, cuenta.Saldo);
        return ApiResponse<RetiroResponse>.Ok(data, "Retiro procesado con éxito.");
    }
}
