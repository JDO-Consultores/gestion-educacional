using System;

namespace Gestionador.Reports.Responses
{
    public class AdquisicionPagoResponse
    {
        public int ID { get; set; }
        public int FormaPagoID { get; set; }
        public int? BancoID { get; set; }
        public int NroRecaudacion { get; set; }
        public string NroCheque { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public DateTime FechaRegistro { get; set; }
        public FormasPagoResponse FormaPagoResponse { get; set; }
        public BancosResponse BancoResponse { get; set; }
        public string FormaPago => FormaPagoResponse.FormaPago;
        public string Banco => BancoResponse?.Banco;
    }
}