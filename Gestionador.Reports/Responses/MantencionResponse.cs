namespace Gestionador.Reports.Responses
{
    public class MantencionResponse
    {
        public int ID { get; set; }
        public int CategoriaID { get; set; }
        public string Mantencion { get; set; }
        public bool IsActive { get; set; }
        public string Categoria { get; set; }

        public CategoriasResponse Categorias { get; set; }
    }
}