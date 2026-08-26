using System.Collections.Generic;

namespace Gestionador.Responses
{
    public class ConceptoResponse
    {
        public int ID { get; set; }
        public int CategoriaID { get; set; }
        public string Concepto { get; set; }
        public bool IsActive { get; set; }
        public bool IsNicho {  get; set; }
        public List<SeccionConceptosResponse> SeccionConceptos { get; set; } = new List<SeccionConceptosResponse>();
        public List<ServiciosConceptosResponse> ServiciosConceptos { get; set; } = new List<ServiciosConceptosResponse>();
        public string Text => Concepto;
    }
}