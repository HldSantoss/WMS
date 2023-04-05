using System.ComponentModel.DataAnnotations;

namespace Api.Validations
{
    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == default)
                return false;

            if ((long)value <= 0)
                return false;

            return true;
        }
    }
}