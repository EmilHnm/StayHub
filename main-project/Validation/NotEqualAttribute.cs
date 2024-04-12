namespace Validation;

using System.ComponentModel.DataAnnotations;

public class NotEqualAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public NotEqualAttribute(string otherProperty)
    {
        _otherProperty = otherProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_otherProperty) ?? throw new ArgumentException("Property with this name not found");
        var otherValue = property.GetValue(validationContext.ObjectInstance, null);

        if (value != null && value.Equals(otherValue))
        {
            return new ValidationResult(ErrorMessage ?? "The two properties cannot be equal.");
        }

        return ValidationResult.Success;
    }
}