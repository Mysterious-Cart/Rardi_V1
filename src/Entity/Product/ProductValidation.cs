namespace CHKS.Validator;

using FluentValidation;
using Entity;

public class ProductValidator : AbstractValidator<CreateProductRequest>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");

        RuleFor(x => x.Name)
            .Length(3, 50)
            .WithMessage("Name must be between 3 and 50 characters");

        RuleFor(x => x.Name)
            .Matches(@"^[a-zA-Z0-9\s]*$")
            .WithMessage("Name must contain only letters and numbers");

        RuleFor(x => x.Name)
            .Must(x => x != "Product")
            .WithMessage("Name cannot be 'Product'");

        RuleFor(x => x.Stock)
            .NotEmpty()
            .WithMessage("Stock is required");

        RuleFor(x => x.Stock)
            .GreaterThan(0)
            .WithMessage("Stock must be greater than 0");
        RuleFor(x => x.Stock)
            .LessThan(1000)
            .WithMessage("Stock must be less than 1000");

        RuleFor(x => x.Import)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Import cannot be negative");

        RuleFor(x => x.Export)
            .NotEmpty()
            .WithMessage("Export is required");

        RuleFor(x => x.Export)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Export cannot be negative");

        RuleFor(x => x.Export)
            .LessThan(x => x.Import)
            .WithMessage("Export cannot be greater than Import");

        RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required");
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status not recognized");
        
        RuleFor(x => x.Setting)
            .NotNull()
            .WithMessage("Setting cannot be null");
    }
}