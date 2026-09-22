using BancoCore.Common;
using BancoCore.Models.Request;
using BancoCore.Models.Response;

namespace BancoCore.Services;

public interface IBancoService
{
    Task<ApiResponse<ConsultaCuentaResponse>> ObtenerCuentaPorId(ConsultaCuentaRequest request);
    Task<ApiResponse<DepositoResponse>> Depositar(DepositoRequest request);
    Task<ApiResponse<RetiroResponse>> Retirar(RetiroRequest request);
}