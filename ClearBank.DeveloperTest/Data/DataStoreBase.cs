using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    public abstract class DataStoreBase
    {
        public abstract Account GetAccount(string accountNumber);
        public abstract void UpdateAccount(Account account);
    }
}
