using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace Eleve_Backend.Infrastructure.Documents
{
    public class InvoiceDocument : IDocument
    {
        public Order Order { get; }

        public InvoiceDocument(Order order)
        {
            Order=order;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontFamily(Fonts.Arial).FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("ELEVÉ STUDIO").FontSize(20).SemiBold().FontColor(Colors.Indigo.Medium);

                   
                    column.Item().Text("Artisanal Footwear Archive".ToUpper()).FontSize(9).FontColor(Colors.Grey.Medium);

                    column.Item().PaddingTop(10).Text("5th Avenue Archive District");
                    column.Item().Text("New York, NY 10001");
                });

                row.ConstantItem(150).Column(column =>
                {
                    column.Item().AlignRight().Text("INVOICE").FontSize(16).SemiBold();

                    // Safe null checks
                    var refNumber = Order.OrderReference ?? Order.Id.ToString();
                    column.Item().AlignRight().Text($"#{refNumber}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    column.Item().AlignRight().Text($"{Order.OrderDate:MMM dd, yyyy}");
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                // Billing Info
                column.Item().PaddingBottom(20).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("BILL TO").FontSize(9).Bold().FontColor(Colors.Grey.Medium);

                        var name = Order.ShippingAddress?.Name ?? "Valued Client";
                        var city = Order.ShippingAddress?.City ?? "Unknown Location";

                        c.Item().Text(name).Bold();
                        c.Item().Text(city);
                    });
                });

                // Table
                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Items").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                        header.Cell().AlignRight().Text("QTY").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                        header.Cell().AlignRight().Text("UNIT PRICE").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);
                        header.Cell().AlignRight().Text("TOTAL").Bold().FontSize(9).FontColor(Colors.Grey.Darken2);

                        header.Cell().ColumnSpan(4).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    });

                    if (Order.Items != null)
                    {
                        foreach (var item in Order.Items)
                        {
                            table.Cell().PaddingVertical(10).Text(item.ProductName).SemiBold();
                            table.Cell().PaddingVertical(10).AlignRight().Text(item.Quantity.ToString());
                            table.Cell().PaddingVertical(10).AlignRight().Text($"${item.UnitPrice:N2}");
                            table.Cell().PaddingVertical(10).AlignRight().Text($"${(item.Quantity * item.UnitPrice):N2}").Bold();

                            table.Cell().ColumnSpan(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                        }
                    }
                });

                // Totals
                column.Item().PaddingTop(20).AlignRight().Column(c =>
                {
                    c.Item().Text($"Grand Total: ${Order.TotalAmount:N2}").FontSize(14).Black().SemiBold();
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Thank you for choosing Elevé Studio. Step into the future.")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Medium);
            });
        }

    }
}
