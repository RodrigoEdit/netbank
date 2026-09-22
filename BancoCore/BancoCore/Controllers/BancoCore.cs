using BancoCore.Models.Request;
using BancoCore.Service;
using BancoCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentasController : ControllerBase
{
    private readonly IBancoService _context;

    public CuentasController(IBancoService context)
    {
        _context = context;
    }

    [HttpGet("cuenta/{id}")]
    public async Task<IActionResult> ObtenerCuenta(int id)
    {
        var resultado = await _context.ObtenerCuentaPorId(new ConsultaCuentaRequest(id));
        if (!resultado.Exito)
            return NotFound(resultado);

        return Ok(resultado);
    }

    [HttpPost("deposito")]
    public async Task<IActionResult> Depositar([FromBody] DepositoRequest request)
    {
        var resultado = await _context.Depositar(request);
        if (!resultado.Exito)
            return BadRequest(resultado);

        return Ok(resultado);
    }

    [HttpPost("retiro")]
    public async Task<IActionResult> Retirar([FromBody] RetiroRequest request)
    {
        var resultado = await _context.Retirar(request);
        if (!resultado.Exito)
            return BadRequest(resultado);

        return Ok(resultado);
    }
}