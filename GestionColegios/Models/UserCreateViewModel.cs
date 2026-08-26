using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionColegios.Models
{
    public class UserCreateViewModel : UserViewModel, IValidatableObject
    {
        // La contraseña es obligatoria al CREAR (en edición es opcional).
        // Las reglas de formato (RegularExpression) y de coincidencia (Compare)
        // se heredan de UserViewModel.
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("La contraseña es obligatoria", new[] { nameof(Password) });
            }

            if (string.IsNullOrWhiteSpace(PasswordConfirm))
            {
                yield return new ValidationResult("La confirmación de la contraseña es obligatoria", new[] { nameof(PasswordConfirm) });
            }
        }
    }
}