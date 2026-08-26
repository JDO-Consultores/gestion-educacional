using System.Collections.Generic;

namespace Gestionador.Reports.Models
{
    public class FichaViewModelBase
    {
        public int CementerioID { get; set; }
        public int ProductoID { get; set; }
        public int UbiPatio { get; set; }
        public int UbiSector { get; set; }
        public int UbiNumero { get; set; }
        public bool IsRegulated { get; set; }
        public string Observacion { get; set; }
        public PersonaViewModel Comprador { get; set; }
        public PersonaViewModel Referente { get; set; }
        public List<ServicioViewModel> Servicios { get; set; } = new List<ServicioViewModel>();
        public List<DifuntoViewModel> Difuntos { get; set; } = new List<DifuntoViewModel>();
        public List<FormaPagoViewModel> FormaPagos { get; set; } = new List<FormaPagoViewModel>();
    }
}