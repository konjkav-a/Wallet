using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{

    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }

        // Credit | Debit | TransferIn | TransferOut
        public string Type { get; set; } = string.Empty; 
        public decimal Amount { get; set; }
        public string? Reference { get; set; }
        public DateTime CreatedAt { get; set; }


        public Transaction(Guid id, Guid walletId, string type, decimal amount, string? reference = null)
        {
            Id = id;
            WalletId = walletId;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Amount = amount;
            Reference = reference;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
