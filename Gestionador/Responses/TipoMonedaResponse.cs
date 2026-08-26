namespace Gestionador.Responses
{
    public class TipoMonedaResponse
    {
        public int ID { get; set; }
        public string TipoMoneda { get; set; }
        public bool IsActive { get; set; }
        public string Periodicidad { get; set; }
        public string Format { get; set; }
        public bool HasSymbol { get; set; }
        public string TipoMonedaTexto => TipoMoneda;
    }
}