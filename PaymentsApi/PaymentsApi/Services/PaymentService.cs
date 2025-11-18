using Microsoft.EntityFrameworkCore;
using PaymentsApi.Data;
using PaymentsApi.Dtos;
using PaymentsApi.Models;

namespace PaymentsApi.Services
{
    public class PaymentService :IPaymentService
    {
        private readonly PaymentsDbContext _db;

        public PaymentService(PaymentsDbContext db)
        {
            _db = db;
        }
        public async Task<Payment> CreatePaymentAsync(CreatePaymentRequest request)
        {
            if (request.Amount <= 0) throw new ArgumentException("Amount must be > 0");

            // Idempotency check
            var existing = await _db.Payments.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ClientRequestId == request.ClientRequestId);

            if (existing != null) return existing;

            // Transaction scope - ensure sequence increment and insert are atomic
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var dateKey = DateTime.UtcNow.ToString("yyyyMMdd"); // server UTC date used for reference

                      

                var seqNum = 1;
                var reference = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{seqNum.ToString().PadLeft(4, '0')}";

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    Reference = reference,
                    Amount = request.Amount,
                    Currency = request.Currency,
                    CreatedAt = DateTime.UtcNow,
                    ClientRequestId = request.ClientRequestId
                };

                _db.Payments.Add(payment);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return payment;
            }
            catch (DbUpdateException)
            {
                // possible concurrency/unique constraint issue - try to get existing by clientRequestId
                var existingAfter = await _db.Payments.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.ClientRequestId == request.ClientRequestId);
                if (existingAfter != null) return existingAfter;
                throw; // rethrow if not found
            }
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Payment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Payment?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Payment?> UpdateAsync(Guid id, CreatePaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
