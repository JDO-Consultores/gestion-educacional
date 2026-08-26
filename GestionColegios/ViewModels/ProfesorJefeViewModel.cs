using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionColegios.ViewModels
{
    public class ProfesorJefeViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "El RUT es obligatorio.")]
        public string Rut { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Email { get; set; }

        public string Telefono { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        public int EstadoProfesorID { get; set; }
        public string EstadoProfesor { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public bool Vigente { get; set; } = true;

        // Cantidad de cursos activos asignados (para el listado)
        public int CursosAsignados { get; set; }

        // Lista para el formulario
        public List<SelectItemViewModel> EstadosDisponibles { get; set; } = new List<SelectItemViewModel>();
    }
}
