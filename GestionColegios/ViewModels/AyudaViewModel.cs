using System.Collections.Generic;

namespace GestionColegios.ViewModels
{
    /// <summary>
    /// Grupo de temas mostrados en el sidebar del Centro de Ayuda.
    /// </summary>
    public class AyudaGrupoViewModel
    {
        public string Grupo { get; set; }
        public List<AyudaTemaViewModel> Items { get; set; } = new List<AyudaTemaViewModel>();
    }

    /// <summary>
    /// Tema individual del Centro de Ayuda (asociado a un archivo .md en /docs).
    /// </summary>
    public class AyudaTemaViewModel
    {
        public string Titulo { get; set; }
        public string Archivo { get; set; }
    }
}
