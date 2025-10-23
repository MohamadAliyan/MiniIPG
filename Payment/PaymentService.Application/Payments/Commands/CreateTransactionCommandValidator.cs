using FluentValidation;

namespace PaymentService.Application.Payments.Commands;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithMessage("  مبلغ تراکنش باید بزرگتر از 0 باشد.");
        RuleFor(x => x.PhoneNumber).Length(11).WithMessage(" شماره موبایل نامعتبر است");
        RuleFor(x => x.TerminalNo).NotEmpty().WithMessage("TerminalNo الزامی است.");
        RuleFor(x => x.ReservationNumber)
            .NotEmpty().WithMessage("ReservationNumber الزامی است.");
        RuleFor(x => x.RedirectUrl)
            .NotEmpty().WithMessage("RedirectUrl الزامی است.")
            .Must(IsValidUrl).WithMessage("RedirectUrl باید یک آدرس معتبر با http یا https باشد.");

    
}
    private bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}