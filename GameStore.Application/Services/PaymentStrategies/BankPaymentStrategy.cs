using GameStore.Application.DTOs.OrderDtos;
using GameStore.Domain.Entities;
using GameStore.Domain.Enums;
using GameStore.Domain.Settings;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GameStore.Application.Services.PaymentStrategies;
public class BankPaymentStrategy(IOptions<InvoiceSettings> options) : IPaymentStrategy
{
    public Task<PaymentResult> ProcessPaymentAsync(Order cart, PaymentRequestDto request)
    {
        var creationDate = DateTime.UtcNow;
        var validUntil = DateTime.UtcNow.AddDays(options.Value.ValidityDays);
        var cartSum = cart
                .OrderGames
                .Sum(og => og.Price * og.Quantity * (100 - og.Discount) / 100);

        var pdfDocument = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.Content().Column(column =>
                {
                    column.Item().Text("Invoice").FontSize(20).Bold().AlignCenter();
                    column.Item().Text($"User ID: {cart.CustomerId}");
                    column.Item().Text($"Order ID: {cart.Id}");
                    column.Item().Text($"Creation Date: {creationDate:dd-MM-yyyy}");
                    column.Item().Text($"Valid Until: {validUntil:dd-MM-yyyy}");
                    column.Item().Text($"Sum: ${cartSum:F2}");
                });
            });
        });

        using var memoryStream = new MemoryStream();
        pdfDocument.GeneratePdf(memoryStream);
        return Task.FromResult(new PaymentResult
        {
            ResultType = PaymentResultType.InvoiceGenerated,
            Invoice = memoryStream.ToArray(),
        });
    }
}
