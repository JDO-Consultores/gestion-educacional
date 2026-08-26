using System;

namespace Gestionador.Responses
{
    public class ConceptosReportResponse
    {
        public int Count { get; set; }
        public string Categoria { get; set; }
        public string Concepto { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaRegistro { get; set; }

        public string FullData => $"({Count}) {Concepto}";
        public DateTime Fecha => FechaRegistro.Date;

    }
}