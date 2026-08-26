using System;
using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class ValorMonedaRequest
    {
        public int? ID { get; set; }
        public int TipoMonedaID { get; set; }
        [Required(ErrorMessage = "El valor de la moneda es obligatorio.")]
        public decimal Valor { get; set; }
        [Required(ErrorMessage = "La fecha es obligatorio.")]
        public DateTime Fecha { get; set; }
    }
}