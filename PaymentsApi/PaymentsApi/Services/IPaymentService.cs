using PaymentsApi.Dtos;
using PaymentsApi.Models;
namespace PaymentsApi.Services
{
    public interface IPaymentService
    {
        
            Task<Payment> CreatePaymentAsync(CreatePaymentRequest request);
            Task<IEnumerable<Payment>> GetAllAsync();
            Task<Payment?> GetByIdAsync(Guid id);
            Task<Payment?> UpdateAsync(Guid id, CreatePaymentRequest request);
            Task<bool> DeleteAsync(Guid id);
        
    }
}
