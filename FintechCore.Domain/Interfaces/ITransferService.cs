using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Interfaces
{
    public interface ITransferService
    {
        Task<TransferResult> TransferAsync(int fromAccountId, int toAccountId, decimal amount);
    }

    public class TransferResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TransactionId { get; set; }
        public decimal FromBalance { get; set; }
        public decimal ToBalance { get; set; }
    }
}
