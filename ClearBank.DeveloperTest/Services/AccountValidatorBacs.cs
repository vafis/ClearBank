using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Contracts;
using ClearBank.DeveloperTest.Types;


namespace ClearBank.DeveloperTest.Services
{
    public class AccountValidatorBacs : IAccountValidator<AccountValidatorBacs>
    {
        public bool Validate(MakePaymentRequest makePaymentRequest, Account account)
        {
            bool result = true;
            if (account == null)
            {
                result = false;
            }
            else if (!account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
            {
                result = false;
            }
            return result;
        }
    }
}
