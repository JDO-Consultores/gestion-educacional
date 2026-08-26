using System;

namespace Gestionador.Responses
{
    public class ValorMonedasResponse
    {
        public int ID { get; set; }
        public decimal Valor { get; set; }
        public int Day { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime Fecha { get; set; }
        public bool IsActive { get; set; }
        public TipoMonedaResponse TipoMonedaResponse { get; set; } = new TipoMonedaResponse();
    }
}