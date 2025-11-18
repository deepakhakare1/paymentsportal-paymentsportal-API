using System.ComponentModel.DataAnnotations;
namespace PaymentsApi.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Reference { get; set; } = null!; // PAY-YYYYMMDD-####

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClientRequestId { get; set; } = null!;
    }
}
