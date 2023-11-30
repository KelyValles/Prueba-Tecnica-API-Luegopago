using System.ComponentModel.DataAnnotations;

namespace API_LuegoPago.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidDayAttribute : ValidationAttribute
    {
        private static readonly string[] ValidDays = { "lunes", "martes", "miércoles", "jueves", "viernes" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var day = value as string;

            if (string.IsNullOrEmpty(day) || !ValidDays.Contains(day.ToLower()))
            {
                return new ValidationResult("texto inválido. Por favor, elija entre lunes, martes, miércoles, jueves o viernes.");
            }

            return ValidationResult.Success;
        }
    }
}
