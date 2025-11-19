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

                // Load or create DailySequence row
                var seq = await _db.DailySequences
                    .FirstOrDefaultAsync(s => s.DateKey == dateKey);

                if (seq == null)
                {
                    seq = new DailySequence { DateKey = dateKey, LastSequence = 0 };
                    _db.DailySequences.Add(seq);
                    await _db.SaveChangesAsync();
                }

                // Increment sequence
                seq.LastSequence += 1;
                _db.DailySequences.Update(seq);
                await _db.SaveChangesAsync();

                var seqNum = seq.LastSequence;
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment == null) return false;
            _db.Payments.Remove(payment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _db.Payments.AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _db.Payments.FindAsync(id);
        }

        public async Task<Payment?> UpdateAsync(Guid id, CreatePaymentRequest request)
        {
            var payment = await _db.Payments.FindAsync(id);
            if (payment == null) return null;

            if (request.Amount <= 0) throw new ArgumentException("Amount must be > 0");

            // Do not allow clientRequestId or reference or createdAt edits (business choice)
            payment.Amount = request.Amount;
            payment.Currency = request.Currency;

            _db.Payments.Update(payment);
            await _db.SaveChangesAsync();
            return payment;
        }
    }
}
