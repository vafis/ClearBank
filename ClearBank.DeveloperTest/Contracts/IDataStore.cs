using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Contracts
{
    public interface IDataStore
    {
          Account GetAccount(string accountNumber);
          void UpdateAccount(Account account);
    }
}
