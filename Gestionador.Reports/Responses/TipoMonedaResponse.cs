namespace Gestionador.Reports.Responses
{
    public class TipoMonedaResponse
    {
        public int ID { get; set; }
        public string TipoMoneda { get; set; }
        public bool IsActive { get; set; }
        public string Periodicidad { get; set; }

        public string TipoMonedaTexto => TipoMoneda;
    }
}
