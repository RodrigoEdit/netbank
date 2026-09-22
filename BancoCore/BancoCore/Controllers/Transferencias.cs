using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BancoCore.Data;
using BancoCore.Models;

namespace BancoCore.Controllers;

public record TransferenciasRequest(string CuentaOrigen, string CuentaDestino, decimal Monto);

[ApiController]
[Route("api/[controller]")]
public class TransferenciasController : ControllerBase
{
    private readonly BancoDbContext _context;

    public TransferenciasController(BancoDbContext context)
    {
        _context = context;
    }

    [HttpPost("transaccion")]
    public async Task<IActionResult> Transferir([FromBody] TransferenciasRequest request)
    {
        if (request.Monto <= 0)
        {
            return BadRequest(new { error = "El monto a transferir debe ser mayor a 0." });
        }
        var cuentaOrigen = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == request.CuentaOrigen);
        var cuentaDestino = await _context.Cuentas.FirstOrDefaultAsync(c => c.NumeroCuenta == request.CuentaDestino);
        if (cuentaOrigen == null || cuentaDestino == null)
        {
            return NotFound(new { error = "Una o ambas cuentas no fueron encontradas." });
        }
        if (cuentaOrigen.Saldo < request.Monto)
        {
            return BadRequest(new { error = "Saldo insuficiente en la cuenta de origen." });
        }
        cuentaOrigen.Saldo -= request.Monto;
        cuentaDestino.Saldo += request.Monto;

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
        return Ok(new
        {
            mensaje = "Transferencia procesada con éxito",
            montoTransferido = request.Monto,
            saldoCuentaOrigen = cuentaOrigen.Saldo,
            saldoCuentaDestino = cuentaDestino.Saldo
        });
    }

    [HttpGet("historial/{cuenta}")]
    public async Task<IActionResult> ObtenerHistorialTransacciones(string cuenta)
    {
        var cuentaExiste = await _context.Cuentas.AnyAsync(c => c.NumeroCuenta == cuenta);
        if (!cuentaExiste)
        {
            return BadRequest(new { error = "El número de cuenta no existe." });
        }
        var transacciones = await _context.Transaccion.Where(t => t.NumeroCuentaOrigen == cuenta || t.NumeroCuentaDestino == cuenta)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();

        return Ok(transacciones);
    }

}
