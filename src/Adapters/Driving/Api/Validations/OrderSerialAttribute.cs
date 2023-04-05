using System.ComponentModel.DataAnnotations;

namespace Api.Validations
{
    public class OrderSerialAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == default)
                return false;

            if ((double)value <= 0.0)
                return false;

            return true;
        }
    }
}