using System.ComponentModel.DataAnnotations;

namespace ShopFlow.Features.AddToCart;

public static class AddToCartValidator
{
    public static (bool IsValid, List<string> Errors) Validate(AddToCartRequest request)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);

        var errors = validationResults
            .Where(vr => !string.IsNullOrWhiteSpace(vr.ErrorMessage))
            .Select(vr => vr.ErrorMessage!)
            .ToList();

        return (isValid, errors);
    }
}
