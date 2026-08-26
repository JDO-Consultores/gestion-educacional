using Gestionador.Models;
using System.Collections.Generic;

namespace Gestionador.Responses
{
    public class AdquisicionResponse : AdquisicionBase
    {
        public int? TranferredID { get; set; }
        public int? NextTransferID { get; set; }
        public int ProductoID { get; set; }
        public bool IsTranferred { get; set; }
        public bool IsRegulated { get; set; }
        public string TranferredNroFicha { get; set; }
        public string NextTransferNroFicha { get; set; }
        public string UbiPatio { get; set; }
        public string UbiSector { get; set; }
        public string UbiNumero { get; set; }
        public string Producto { get; set; }
        public string LetraNicho { get; set; }
        public decimal PrecioProducto { get; set; }
        public string Observacion { get; set; }
        public string Estado { get; set; }
        public SectoresResponse Sector { get; set; }
        public SeccionConceptosResponse SeccionConceptosResponse { get; set; }
        public PersonaResponse Comprador { get; set; }
        public PersonaResponse Referente { get; set; }
        public List<DifuntoViewModel> Difuntos { get; set; } = new List<DifuntoViewModel>();
        public List<AdquisicionServiciosResponse> Servicios { get; set; } = new List<AdquisicionServiciosResponse>();
        public List<AdquisicionMantencionResponse> Mantenciones {  get; set; } = new List<AdquisicionMantencionResponse>();
        public List<AdquisicionPagoResponse> FormaPagos { get; set; } = new List<AdquisicionPagoResponse>();
        public List<AdquisicionObservacion> Observaciones { get; set; } = new List<AdquisicionObservacion>();
        public List<HistorialResponse> Historials { get; set; } = new List<HistorialResponse>();
    }
}