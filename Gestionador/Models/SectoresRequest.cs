using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class SectoresRequest
    {
        public int ID { get; set; }
        [StringLength(10, ErrorMessage = "El sector debe ser máximo 10 caracteres.")]
        public string Sector { get; set; }
    }
}