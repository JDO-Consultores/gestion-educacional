using System.Web.Mvc;

namespace GestionColegios.Helpers
{
    public static class RutHtmlHelper
    {
        /// <summary>
        /// Devuelve el RUT formateado con puntos y guión listo para mostrar en Razor.
        /// Uso: @Html.Rut(Model.Rut)
        /// </summary>
        public static MvcHtmlString Rut(this HtmlHelper helper, string rut)
        {
            return new MvcHtmlString(RutHelper.Formatear(rut));
        }
    }
}
