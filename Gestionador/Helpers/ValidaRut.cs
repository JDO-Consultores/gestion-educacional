using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Gestionador.Helpers
{
    public class ValidaRut : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool validacion = false;
            try
            {
                if (value != null)
                {
                    string rut = value.ToString();
                    rut = rut.ToUpper().Replace(".", "").Replace("-", "");

                    if (rut.Length < 2) return false;

                    int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));
                    char dv = char.Parse(rut.Substring(rut.Length - 1, 1));

                    int m = 0, s = 1;
                    for (; rutAux != 0; rutAux /= 10)
                    {
                        s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                    }

                    validacion = (dv == (char)(s != 0 ? s + 47 : 75));
                }
            }
            catch (Exception)
            {
                return false;
            }
            return validacion;
        }

        public static string Parse(string rut)
        {
            if (string.IsNullOrWhiteSpace(rut) || rut.Length <= 1) return string.Empty;

            var dv = rut.Substring(rut.Length - 1).ToUpper();
            var sub = rut.Substring(0, rut.Length - 1).Replace(".", "").Replace("-", "").Trim();

            if (long.TryParse(sub, out long parsedNumber))
            {
                var culture = new CultureInfo("es-CL");
                return $"{parsedNumber.ToString("N0", culture)}-{dv}";
            }
            return string.Empty;
        }
    }
}