using FluentValidation;
using Menoo.PrinterService.Client.DTOs;
using Menoo.PrinterService.Client.Properties;

namespace Menoo.PrinterService.Client.Validators
{
    public class UpdatePreferencesPrinterRequestValidator : AbstractValidator<UpdatePrinterPreferencesRequest>
    {
        public UpdatePreferencesPrinterRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(AppMessages.SectorStoreEmpty)
                .MaximumLength(25)
                .WithMessage(AppMessages.SectorStoreMaxLength);

            RuleFor(x => x.Printer)
                .NotEmpty()
                .WithMessage(AppMessages.PrinterEmpty)
                .Must(PrinterSelection)
                .WithMessage(AppMessages.PrinterEmpty);

            RuleFor(x => x.PrintEvents)
                .NotEmpty()
                .WithMessage(AppMessages.PrintEventsEmpty);

            RuleFor(x => x.Copies)
                .GreaterThan(0)
                .WithMessage(AppMessages.CopiesZero);
        }

        private bool PrinterSelection(string value)
        {
            return value != AppMessages.Empty;
        }
    }
}
