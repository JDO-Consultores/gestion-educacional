namespace Gestionador.Responses
{
    public class ServiciosConceptosResponse
    {
        public int ID { get; set; }
        public int ServicioID { get; set; }
        public int ConceptoID { get; set; }
        public int SeccionID { get; set; }
        public int TipoMonedaID { get; set; }
        public bool IsActive { get; set; }
        public decimal Precio { get; set; }
        public ServiciosResponse Servicios { get; set; }        
        public ConceptoResponse Concepto { get; set; }
        public SectoresResponse Sectores { get; set; }
        public TipoMonedaResponse TipoMoneda { get; set; }
        public string Text => Servicios.Servicio;
        public string Servicio => Servicios.Servicio;
        public string Categoria => Servicios.Categoria;
        public string TipoMonedas => TipoMoneda.TipoMoneda;
    }
}