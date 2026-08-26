using System;

namespace Gestionador.Reports.Responses
{
    public class AdquisicionBase
    {
        public int ID { get; set; }
        public string NroFicha { get; set; }
        public int CementerioID { get; set; }
        public string CompNombre { get; set; }
        public string CompApellido { get; set; }
        public string CompRut { get; set; }
        public string RefNombre { get; set; }
        public string RefApellido { get; set; }
        public string Cementerio { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public int TipoMonedaID { get; set; }
        public Nullable<decimal> ValorTipoMonedaActual { get; set; }
        public Nullable<decimal> PrecioTipoMoneda { get; set; }
        public Nullable<DateTime> FechaTipoMoneda { get; set; }
        public TipoMonedaResponse TipoMoneda { get; set; }
        public string CompNombreApellido => $"{CompApellido} {CompNombre}";
        public string RefNombreApellido => $"{RefApellido} {RefNombre}";
        public string FechaLocal => FechaAdquisicion.ToString("dd-MM-yyyy");
    }
}