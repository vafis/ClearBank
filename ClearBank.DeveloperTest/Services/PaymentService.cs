using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;
using ClearBank.DeveloperTest.Contracts;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private IDataStoreFactory _dataStoreFactory;
        private IAccountValidator<AccountValidatorBacs> _accountValidatorBacs;
        private IAccountValidator<AccountValidatorFasterPayments> _accountValidatorFasterPayments;
        private IAccountValidator<AccountValidatorChaps> _accountValidatorChaps;
        private IAppConfig _appConfig;
        public PaymentService(IDataStoreFactory dataStoreFactory, 
                              IAccountValidator<AccountValidatorBacs> accountValidatorBacs,
                              IAccountValidator<AccountValidatorFasterPayments> accountValidatorFasterPayments,
                              IAccountValidator<AccountValidatorChaps> accountValidatorChaps,
                              IAppConfig appConfig)
        {
            _dataStoreFactory = dataStoreFactory;
            _accountValidatorBacs  = accountValidatorBacs;
            _accountValidatorFasterPayments = accountValidatorFasterPayments;
            _accountValidatorChaps = accountValidatorChaps;
            _appConfig = appConfig;
        }
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var datastore = _dataStoreFactory.Create(_appConfig.GetKeyValue("DataStoreType"));
            var account = datastore.GetAccount(request.DebtorAccountNumber);

            var result = new MakePaymentResult();

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                    result.Success = _accountValidatorBacs.Validate(request, account);
                    break;
                case PaymentScheme.FasterPayments:
                    result.Success = _accountValidatorFasterPayments.Validate(request, account);
                    break;
                case PaymentScheme.Chaps:
                    result.Success = _accountValidatorChaps.Validate(request, account);
                    break;
            }
            

            if (result.Success)
            {
                account.Balance -= request.Amount;
                datastore.UpdateAccount(account);     
            }

            return result;
        }
    }
}
