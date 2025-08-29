using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    
    public interface IWalletRepository
    {
        bool TryAddWallet(WalletAggregate aggregate);
        WalletAggregate? GetWallet(Guid id);
    }
}
