using Bookings.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookings.Infrastructure.Services;
public interface IPaymentServiceFactory
{
    IPaymentService CreatePaymentService(string paymentMethod);
}
