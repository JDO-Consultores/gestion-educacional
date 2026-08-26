using System;

namespace Gestionador.Models
{
    public class FormaPagoViewModel
    {
        public int ID { get; set; }
        public int FormaPagoID { get; set; }
        public int NroRecaudacion { get; set; }
        public int Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string NroCheque { get; set; }
        public int? BancoID { get; set; }
    }
}