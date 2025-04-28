using Bookings.Domain.AggregatesModel.PaymentAggregate;
using Bookings.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly BookingContext _context;

    public PaymentRepository(BookingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IUnitOfWork UnitOfWork => _context;

    public Payment Add(Payment payment)
    {
        return _context.Payments.Add(payment).Entity;
    }

    public async Task<Payment?> GetAsync(int paymentId)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == paymentId);
    }

    public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
    {
        return await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
    }

    public async Task<IEnumerable<Payment>> GetByBookingAsync(int bookingId)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(p => p.BookingId == bookingId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByUserAsync(int userId)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStatusAsync(string status)
    {
        return await _context.Payments
            .AsNoTracking()
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }
}
