using System.Collections.Generic;

namespace GestionColegios.ViewModels
{
    /// <summary>Item de plantilla para listar en el modal de generación y en el mantenedor.</summary>
    public class PlantillaCertificadoItemViewModel
    {
        public int ID { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string NombreArchivo { get; set; }
        public bool TienePlantilla { get; set; }
        public int? FirmanteDefectoID { get; set; }
        public string FirmanteDefecto { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>Item de firmante para selects y mantenedor.</summary>
    public class FirmanteItemViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>Datos del establecimiento administrables desde el mantenedor.</summary>
    public class EstablecimientoViewModel
    {
        public int ID { get; set; }
        public string RBD { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public int? RegionID { get; set; }
        public int? ComunaID { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string SitioWeb { get; set; }
        public bool TieneLogo { get; set; }
    }

    /// <summary>Payload para guardar un firmante (crear/editar).</summary>
    public class FirmanteGuardarViewModel
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
