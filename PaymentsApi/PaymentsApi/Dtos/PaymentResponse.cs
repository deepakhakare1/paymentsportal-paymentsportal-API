
using PaymentsApi.Models;
namespace PaymentsApi.Dtos
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public string Reference { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string ClientRequestId { get; set; } = null!;

        public static PaymentResponse FromModel(Payment p) => new PaymentResponse
        {
            Id = p.Id,
            Reference = p.Reference,
            Amount = p.Amount,
            Currency = p.Currency,
            CreatedAt = p.CreatedAt,
            ClientRequestId = p.ClientRequestId
        };

    }
}
