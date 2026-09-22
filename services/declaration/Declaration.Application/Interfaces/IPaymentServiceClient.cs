using System.Threading.Tasks;
using System;

namespace Declaration.Application.Interfaces;

public interface IPaymentServiceClient
{
    Task<string> GetPaymentStatusAsync(string paymentReference, Guid tenantId);
}
