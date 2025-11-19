using System.ComponentModel.DataAnnotations;
namespace PaymentsApi.Dtos
{
    public class CreatePaymentRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("^(USD|EUR|INR|GBP)$", ErrorMessage = "Currency must be USD, EUR, INR, or GBP")]
        public string Currency { get; set; } = null!;

        // clientRequestId must be provided by client (GUID recommended)
        [Required]
        [MaxLength(100)]
        public string ClientRequestId { get; set; } = null!;
    }
}
