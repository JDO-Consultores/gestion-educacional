namespace Gestionador.Responses
{
    public class MantencionConceptosResponse
    {
        public int ID { get; set; }
        public int MantencionID { get; set; }
        public int ConceptoID { get; set; }
        public bool IsActive { get; set; }
        public decimal Precio { get; set; }
        public int TipoMonedaID { get; set; }
        public MantencionResponse Mantenciones { get; set; }
        public TipoMonedaResponse TipoMoneda { get; set; }
        public string Text => Mantenciones.Mantencion;
        public string Mantencion => Mantenciones.Mantencion;
        public string TipoMonedas => TipoMoneda.TipoMoneda;
    }
}