using BancoCore.Common;
using BancoCore.Data;
using BancoCore.Models;
using BancoCore.Models.Request;
using BancoCore.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace BancoCore.Services;

public class TransferenciaService : ITransferenciaService
{
    private readonly BancoDbContext _context;

    public TransferenciaService(BancoDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<TransferenciaResultadoResponse>> ProcesarTransferenciaAsync(TransferenciaRequest request)
    {
        if (request.Monto <= 0)
            return ApiResponse<TransferenciaResultadoResponse>.Error("El monto a transferir debe ser mayor a 0.");

        if (request.CuentaOrigen == request.CuentaDestino)
            return ApiResponse<TransferenciaResultadoResponse>.Error("La cuenta de origen y destino no pueden ser iguales.");

        var origen = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == request.CuentaOrigen);
        var destino = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == request.CuentaDestino);

        if (origen == null || destino == null)
            return ApiResponse<TransferenciaResultadoResponse>.Error("Una o ambas cuentas no fueron encontradas.");

        if (origen.Saldo < request.Monto)
            return ApiResponse<TransferenciaResultadoResponse>.Error("Saldo insuficiente en la cuenta de origen.");

        origen.Saldo -= request.Monto;
        destino.Saldo += request.Monto;

        var transaccion = new Transaccion
        {
            NumeroCuentaOrigen = request.CuentaOrigen,
            NumeroCuentaDestino = request.CuentaDestino,
            Monto = request.Monto,
            Fecha = DateTime.UtcNow,
            Tipo = "Transferencia"
        };

        _context.Transaccion.Add(transaccion);
        await _context.SaveChangesAsync();

        var resultado = new TransferenciaResultadoResponse(
            origen.NumeroCuenta,
            destino.NumeroCuenta,
            request.Monto,
            origen.Saldo,
            destino.Saldo
        );

        return ApiResponse<TransferenciaResultadoResponse>.Ok(resultado, "Transferencia procesada con éxito.");
    }
    public async Task<ApiResponse<List<HistorialTransaccionResponse>>> ObtenerHistorialAsync(string numeroCuenta)
    {
        var cuentaExiste = await _context.Cuentas.AnyAsync(c => c.NumeroCuenta == numeroCuenta);
        if (!cuentaExiste)
            return ApiResponse<List<HistorialTransaccionResponse>>.Error("El número de cuenta no existe.");

        var historial = await _context.Transaccion
            .Where(t => t.NumeroCuentaOrigen == numeroCuenta || t.NumeroCuentaDestino == numeroCuenta)
            .OrderByDescending(t => t.Fecha)
            .Select(t => new HistorialTransaccionResponse(
                t.Id,
                t.NumeroCuentaOrigen,
                t.NumeroCuentaDestino,
                t.Monto,
                t.Fecha,
                t.Tipo
            ))
            .ToListAsync();

        return ApiResponse<List<HistorialTransaccionResponse>>.Ok(historial, "Historial obtenido con éxito.");
    }
}