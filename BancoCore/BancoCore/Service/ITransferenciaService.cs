using BancoCore.Common;
using BancoCore.Models;
using BancoCore.Models.Request;
using BancoCore.Models.Response;

namespace BancoCore.Services;

public interface ITransferenciaService
{
    Task<ApiResponse<TransferenciaResultadoResponse>> ProcesarTransferenciaAsync(TransferenciaRequest request);
    Task<ApiResponse<List<HistorialTransaccionResponse>>> ObtenerHistorialAsync(string numeroCuenta);
}