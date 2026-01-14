using Eleve_Backend.Domain.Entities;
namespace Eleve_Backend.Application.Interfaces
{
    public interface IPdfService
    {
        byte[] GenerateInvoice(Order order);
    }
}
