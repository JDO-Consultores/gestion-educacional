using System;

namespace Gestionador.Responses
{
    public class ReportCobranzaResponse
    {
        public int ID { get; set; }
        public string NroFicha { get; set; }
        public string CompNombre { get; set; }
        public string CompApellido { get; set; }
        public string CompRut { get; set; }
        public string CompComuna { get; set; }
        public string CompTelefono { get; set; }
        public string Patio { get; set; }
        public string Sector { get; set; }
        public string UbiNumero { get; set; }
        public string LetraNicho { get; set; }
        public decimal? Deuda { get; set; }

        public string CompNombreApellido => $"{CompApellido} {CompNombre}";
        public string Ubicacion => $"{Patio}-{Sector}-{LetraNicho}{UbiNumero}";

    }
}