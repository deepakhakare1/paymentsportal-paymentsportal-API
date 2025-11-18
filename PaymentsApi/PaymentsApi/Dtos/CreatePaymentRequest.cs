using System.ComponentModel.DataAnnotations;
namespace PaymentsApi.Dtos
{
    public class CreatePaymentRequest
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = null!;

        // clientRequestId must be provided by client (GUID recommended)
        [Required]
        public string ClientRequestId { get; set; } = null!;
    }
}
