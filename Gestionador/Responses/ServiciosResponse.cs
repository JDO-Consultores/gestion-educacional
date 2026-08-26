namespace Gestionador.Responses
{
    public class ServiciosResponse
    {
        public int ID { get; set; }
        public int CategoriaID { get; set; }
        public string Servicio { get; set; }
        public bool IsActive { get; set; }
        public string Categoria { get; set; }

        public PersonaResponse Maestro { get; set; }
        public CategoriasResponse Categorias { get; set; }
    }
}