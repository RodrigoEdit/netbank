
namespace BancoCore.Models;
public class Transaccion
    {
        public int Id { get; set; }
        public string NumeroCuentaOrigen { get; set; } = string.Empty;
        public string NumeroCuentaDestino { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public string Tipo { get; set; } = string.Empty;
    }



