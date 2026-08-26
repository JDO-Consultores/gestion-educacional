using Gestionador.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Gestionador.Models
{
    public class PersonaViewModel
    {
        public int? ID { get; set; }

        [Required(ErrorMessage = "El rut es requerido.")]
        [ValidaRut(ErrorMessage = "El rut ingresado no es valido.")]
        public string Rut { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es requerido.")]
        public string Apellido { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la comuna")]
        public int ComunaID { get; set; }
        [Required(ErrorMessage = "Ingrese la dirección.")]
        public string Direccion1 { get; set; }
        [Required(ErrorMessage = "Ingrese la numeración")]
        public string DirNum { get; set; }
        [Required(ErrorMessage = "Ingrese el teléfono.")]
        public string Telefono { get; set; }
        [EmailAddress(ErrorMessage = "El email es invalido.")]
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
    }
}