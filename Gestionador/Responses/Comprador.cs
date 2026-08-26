namespace Gestionador.Responses
{
    public class PersonaResponse
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Rut { get; set; }
        public int? RegionID { get; set; }
        public int? ComunaID { get; set; }
        public string Direccion1 { get; set; }
        public string DirNum { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public ComunasResponse Comuna { get; set; }

        public string NombreApellido => $"{Nombre} {Apellido}";
        public string FullDireccion => $"{Direccion1}";
        public string NombreComuna => Comuna.Comuna;
    }
}