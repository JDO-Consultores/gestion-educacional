using System;

namespace Gestionador.Responses
{
    public class ReportVentasResponse
    {
        public string FormaPago { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}