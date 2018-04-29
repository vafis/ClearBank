using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Contracts
{
    public interface IAccountValidator<T> 
    {
        bool Validate(MakePaymentRequest makePaymentRequest, Account account);
    }
}
