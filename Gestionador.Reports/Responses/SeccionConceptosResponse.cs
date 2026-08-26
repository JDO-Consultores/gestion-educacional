namespace Gestionador.Reports.Responses
{
    public class SeccionConceptosResponse
    {
        public int ID { get; set; }
        public int SeccionID { get; set; }
        public int ConceptoID { get; set; }
        public int Stock { get; set; }
        public decimal Precio { get; set; }
        public bool IsActive { get; set; }
        public ConceptoResponse Concepto { get; set; }

        public string Text => Concepto.Text;
    }
}