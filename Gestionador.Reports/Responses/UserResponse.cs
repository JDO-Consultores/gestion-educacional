using System;
using System.Collections.Generic;

namespace Gestionador.Reports.Responses
{
    public class UserResponse
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Username { get; set; }
        public DateTime Creado { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

        public List<RolesUsuarios> Roles { get; set; }

        public string NombreApellido => $"{Nombre} {Apellido}";
    }
}