using Microsoft.AspNetCore.Mvc;
using BancoCore.Models.Request;
using BancoCore.Services;

namespace BancoCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferenciasController : ControllerBase
{
    private readonly ITransferenciaService _transferenciaService;

    public TransferenciasController(ITransferenciaService transferenciaService)
    {
        _transferenciaService = transferenciaService;
    }

    [HttpPost("transaccion")]
    public async Task<IActionResult> Transferir([FromBody] TransferenciaRequest request)
    {
        var resultado = await _transferenciaService.ProcesarTransferenciaAsync(request);

        if (!resultado.Exito)
            return BadRequest(resultado);

        return Ok(resultado);
    }

    [HttpGet("historial/{cuenta}")]
    public async Task<IActionResult> ObtenerHistorialTransacciones(string cuenta)
    {
        var resultado = await _transferenciaService.ObtenerHistorialAsync(cuenta);

        if (!resultado.Exito)
            return NotFound(resultado);

        return Ok(resultado);
    }
}