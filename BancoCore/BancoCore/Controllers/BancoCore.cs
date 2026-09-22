using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BancoCore.Data;
using System.Numerics;

namespace BancoCore.Controllers;

public record TransaccionRequest(int num, decimal Monto);

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly BancoDbContext _context;

    public CuentasController(BancoDbContext context)
    {
        _context = context;
    }

    [HttpGet("saldo/{id}")]
    public async Task<IActionResult> ObtenerSaldoPorId(int id)
    {
        var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.Id == id);
        if (cuenta == null)
        {
            return NotFound(new { error = "Cuenta no encontrada." });
        }
        return Ok(cuenta);
    }

    [HttpPost("depositar")]
    public async Task<IActionResult> Depositar([FromBody] TransaccionRequest request)
    {
        if (request.Monto <= 0)
        {
            return BadRequest(new { error = "El monto a depositar debe ser mayor a 0." });
        }
        var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.Id == request.num);
        if (cuenta == null)
        {
            return NotFound(new { error = "Cuenta no encontrada." });
        }
        cuenta.Saldo += request.Monto;
        await _context.SaveChangesAsync();
        return Ok(new
        {
            mensaje = "Depósito procesado con éxito",
            montoDepositado = request.Monto,
            saldoDisponible = cuenta.Saldo
        });
    }

    [HttpPost("retirar")]
    public async Task<IActionResult> Retirar([FromBody] TransaccionRequest request)
    {
        if (request.Monto <= 0)
        {
            return BadRequest(new { error = "El monto a retirar debe ser mayor a 0." });
        }
        var cuenta = await _context.Cuentas.FirstOrDefaultAsync(c => c.Id == request.num);
        if (cuenta == null)
        {
            return NotFound(new { error = "Cuenta no encontrada." });
        }
        if (cuenta.Saldo < request.Monto)
        {
            return BadRequest(new { error = "Saldo insuficiente para realizar la transacción." });
        }
        cuenta.Saldo -= request.Monto;
        await _context.SaveChangesAsync();
        return Ok(new
        {
            mensaje = "Retiro procesado con éxito",
            montoRetirado = request.Monto,
            saldoDisponible = cuenta.Saldo
        });
    }
}