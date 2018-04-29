using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using ClearBank.DeveloperTest.Contracts;
using Xunit;
using Xunit.Extensions;
using Moq;
using Autofac;
using Autofac.Core;

namespace ClearBank.DeveloperTest.Tests
{
     public class VerificationTests
    {
        MakePaymentRequest _makePaymentRequest;
        Account _account;
        public VerificationTests()
        {
            _makePaymentRequest = new MakePaymentRequest();
            _account = new Account()
            {
                AccountNumber = "1111-2222-33333",
                Status = AccountStatus.Disabled,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments
            };
        }

        [Fact]
        public void Verify_Not_DataStore_Account_Update()
        {
            var appConfig = new Mock<IAppConfig>();
            appConfig.Setup(x => x.GetKeyValue(It.IsAny<string>())).Returns("Backup");
            var accountDataStore = new Mock<IDataStore>();
            accountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(_account);

            var factory = new Mock<IDataStoreFactory>();
            factory.Setup(x => x.Create(It.IsAny<string>())).Returns(accountDataStore.Object);


            var accountValidatorBacs = new Mock<IAccountValidator<AccountValidatorBacs>>();
            accountValidatorBacs.Setup(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>())).Returns(false);
            var accountValidatorFasterPayments = new Mock<IAccountValidator<AccountValidatorFasterPayments>>();
            accountValidatorFasterPayments.Setup(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>())).Returns(false);
            var accountValidatorChaps = new Mock<IAccountValidator<AccountValidatorChaps>>();
            accountValidatorChaps.Setup(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>())).Returns(false);

            var sut = new PaymentService(factory.Object, accountValidatorBacs.Object, accountValidatorFasterPayments.Object, accountValidatorChaps.Object, appConfig.Object);
            var ret = sut.MakePayment(new MakePaymentRequest()
            {
                Amount = 100,
                DebtorAccountNumber = "12313-12-12",
                PaymentScheme = PaymentScheme.FasterPayments
            });

            //Verify validations
            accountValidatorBacs.Verify(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>()), Times.Never);
            accountValidatorFasterPayments.Verify(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>()), Times.Once);
            accountValidatorChaps.Verify(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>()), Times.Never);
            //Verify DataStore
            accountDataStore.Verify(x => x.UpdateAccount(It.IsAny<Account>()), Times.Never);
            //


        }

    
    }
}
