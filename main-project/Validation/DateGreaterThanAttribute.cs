using System;
using System.ComponentModel.DataAnnotations;

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _otherProperty;
    private readonly int _numberOfDays;

    public DateGreaterThanAttribute(string otherProperty, int numberOfDays = 0)
    {
        _otherProperty = otherProperty;
        _numberOfDays = numberOfDays;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var property = validationContext.ObjectType.GetProperty(_otherProperty);

        if (property == null)
        {
            throw new ArgumentException("Property with this name not found");
        }

        var otherValue = property.GetValue(validationContext.ObjectInstance, null);

        if (value != null && otherValue != null)
        {
            DateTime d1 = (DateTime)value;
            DateTime d2 = ((DateTime)otherValue).AddDays(_numberOfDays);
            if (d1.Date <= d2.Date)
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be greater than {_otherProperty} by {_numberOfDays} days.");
            }
        }

        return ValidationResult.Success;
    }
}
