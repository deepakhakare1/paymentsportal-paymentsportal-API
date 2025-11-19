using System.ComponentModel.DataAnnotations;
namespace PaymentsApi.Models
{
    public class DailySequence
    {
        // Format yyyyMMdd
        [Key]
        [MaxLength(10)]
        public string DateKey { get; set; } = null!;

        public int LastSequence { get; set; }
    }
}
