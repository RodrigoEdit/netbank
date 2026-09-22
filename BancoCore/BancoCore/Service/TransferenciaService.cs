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

        if (string.IsNullOrWhiteSpace(request.CuentaDestino) || request.CuentaOrigen == request.CuentaDestino)
            return ApiResponse<TransferenciaResultadoResponse>.Error("Las cuentas de origen y destino deben ser válidas y diferentes.");

        using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var origen = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == request.CuentaOrigen);
            var destino = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == request.CuentaDestino);

            if (origen == null || destino == null)
                return ApiResponse<TransferenciaResultadoResponse>.Error("Una o ambas cuentas no existen.");

            if (origen.Saldo < request.Monto)
                return ApiResponse<TransferenciaResultadoResponse>.Error("Saldo insuficiente en la cuenta de origen.");

            origen.Saldo -= request.Monto;
            destino.Saldo += request.Monto;

            origen.RowVersion = Guid.NewGuid();
            destino.RowVersion = Guid.NewGuid();

            var movimiento = new Transaccion
            {
                NumeroCuentaOrigen = request.CuentaOrigen,
                NumeroCuentaDestino = request.CuentaDestino,
                Monto = request.Monto,
                Fecha = DateTime.UtcNow,
                Tipo = "Transferencia"
            };

            _context.Transaccion.Add(movimiento);

            await _context.SaveChangesAsync();

            await dbTransaction.CommitAsync();

            var resultado = new TransferenciaResultadoResponse(
                movimiento.NumeroCuentaOrigen,
                movimiento.NumeroCuentaDestino,
                movimiento.Monto,
                origen.Saldo,
                destino.Saldo
            );

            return ApiResponse<TransferenciaResultadoResponse>.Ok(resultado, "Transferencia realizada con éxito.");
        }
        catch (DbUpdateConcurrencyException)
        {
            await dbTransaction.RollbackAsync();
            return ApiResponse<TransferenciaResultadoResponse>.Error("Hubo un conflicto de concurrencia: la cuenta fue modificada por otra operación simultánea. Por favor, intente de nuevo.");
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            return ApiResponse<TransferenciaResultadoResponse>.Error($"Error interno al procesar la transferencia: {ex.Message}");
        }
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