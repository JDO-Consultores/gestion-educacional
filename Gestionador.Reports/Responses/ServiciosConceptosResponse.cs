namespace Gestionador.Reports.Responses
{
    public class ServiciosConceptosResponse
    {
        public int ID { get; set; }
        public int ServicioID { get; set; }
        public int ConceptoID { get; set; }
        public bool IsActive { get; set; }
        public decimal Precio { get; set; }
        public ServiciosResponse Servicios { get; set; }

        public string Text => Servicios.Servicio;
        public string Servicio => Servicios.Servicio;
    }
}