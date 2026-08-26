namespace Gestionador.Responses
{
    public class SeccionConceptosResponse
    {
        public int ID { get; set; }
        public int SeccionID { get; set; }
        public int ConceptoID { get; set; }
        public int Stock { get; set; }
        public int TipoMonedaID { get; set; }
        public decimal Precio { get; set; }
        public bool IsActive { get; set; }
        public ConceptoResponse Concepto { get; set; }
        public SectoresResponse Sectores { get; set; }
        public TipoMonedaResponse TipoMoneda { get; set; }
        public string Text => Concepto.Text;
    }
}