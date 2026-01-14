using Eleve_Backend.Application.Interfaces;
using Eleve_Backend.Domain.Entities;
using Eleve_Backend.Infrastructure.Documents;
using QuestPDF.Fluent;

namespace Eleve_Backend.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateInvoice(Order order)
        {
            var document = new InvoiceDocument(order);
            return document.GeneratePdf();
        }
    }
}
