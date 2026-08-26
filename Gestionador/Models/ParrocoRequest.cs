using Gestionador.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class ParrocoRequest
    {
        public int? ID { get; set; }
        [Required(ErrorMessage = "El rut es requerido.")]
        [ValidaRut(ErrorMessage = "El rut ingresado no es valido.")]
        public string Rut { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        public int TipoAdministradorID { get; set; }
        public bool IsActive { get; set; }
    }
}