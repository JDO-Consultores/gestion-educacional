using System.Web.UI.WebControls;

namespace Gestionador.Responses
{
    public class ParrocosResponse
    {
        public int ID { get; set; }
        public string NombreParroco { get; set; }
        public string RutParroco { get; set; }
        public bool IsActive { get; set; }
        public int TipoAdministradorID { get; set; }    
        public TipoAdministradorResponse TipoAdministrador { get; set; }

        public string Text => $"{NombreParroco} - {TipoAdministrador.TipoAdministrador}";
    }
}