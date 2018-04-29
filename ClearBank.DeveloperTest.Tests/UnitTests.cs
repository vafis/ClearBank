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

namespace ClearBank.DeveloperTest.Tests
{
    public class UnitTests
    {
        MakePaymentRequest _makePaymentRequest;
        Account _account;
        public UnitTests()
        {
            _makePaymentRequest = new MakePaymentRequest();
            _account = new Account()
            {
                AccountNumber = "1111-2222-33333",
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments
            };
        }

        [Theory]
        [InlineData("", typeof(AccountDataStore))]
        [InlineData("anything",  typeof(AccountDataStore))]
        [InlineData("Backup",  typeof(BackupAccountDataStore))]
        public void DataStoreFactory_Test(string dataStore, Type expected)
        {
            //var appConfig = new Mock<IAppConfig>();
            //appConfig.Setup(x => x.GetKeyValue(It.IsAny<string>())).Returns("Backup");
            var sut = new DataStoreFactory();
            var ret = sut.Create(dataStore);
            Assert.IsType(expected, ret);

        }
        [Fact]
        public void AccountValidatorBacs_test()
        {
            var sut = new AccountValidatorBacs();
            _makePaymentRequest.PaymentScheme = PaymentScheme.Bacs;
            var ret = sut.Validate(_makePaymentRequest, _account);
            Assert.True(ret);
            ret = sut.Validate(_makePaymentRequest, null);
            Assert.False(ret);
            _account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            ret = sut.Validate(_makePaymentRequest, _account);
            Assert.False(ret);
        }

        [Fact]
        public void AccountValidatorFasterPayments_test()
        {
            var sut = new AccountValidatorFasterPayments();
            _makePaymentRequest.PaymentScheme = PaymentScheme.FasterPayments;
            _account.Balance = 100;
            _makePaymentRequest.Amount = 50;
            var ret = sut.Validate(_makePaymentRequest, _account);
            Assert.True(ret);
            ret = sut.Validate(_makePaymentRequest, null);
            Assert.False(ret);
            _account.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            ret = sut.Validate(_makePaymentRequest, _account);
            Assert.False(ret);
            _makePaymentRequest.Amount = 50;
            ret = sut.Validate(_makePaymentRequest, _account);
            Assert.False(ret);
        }
        [Fact]
        public void AccountValidatorFasterChaps_test()
        {
            var sut = new AccountValidatorFasterPayments();
            _makePaymentRequest.PaymentScheme = PaymentScheme.Chaps;
            _account.Balance = 100;
            _makePaymentRequest.Amount = 50;
            var ret = sut.Validate(_makePaymentRequest, _account);
            Assert.True(ret);
            ret = sut.Validate(_makePaymentRequest, null);
            Assert.False(ret);
            _account.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;
            ret = sut.Validate(_makePaymentRequest, _account);
            Assert.False(ret);
            _account.Status = AccountStatus.Disabled;
            ret = sut.Validate(_makePaymentRequest, _account);
            Assert.False(ret);
        }

        [Fact]
        public void PaymentService_Test()
        {
            var appConfig = new Mock<IAppConfig>();
            appConfig.Setup(x => x.GetKeyValue(It.IsAny<string>())).Returns("Backup");
            var accountDataStore = new Mock<IDataStore>();
            accountDataStore.Setup(x => x.GetAccount(It.IsAny<string>())).Returns(new Account());
            
            var factory = new Mock<IDataStoreFactory>();
            factory.Setup(x => x.Create(It.IsAny<string>())).Returns(accountDataStore.Object);


            var accountValidatorBacs = new Mock<IAccountValidator<AccountValidatorBacs>>();
            accountValidatorBacs.Setup(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>())).Returns(true);
            var accountValidatorFasterPayments = new Mock<IAccountValidator<AccountValidatorFasterPayments>>();
            accountValidatorFasterPayments.Setup(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>())).Returns(true);
            var accountValidatorChaps = new Mock<IAccountValidator<AccountValidatorChaps>>();
            accountValidatorChaps.Setup(x => x.Validate(It.IsAny<MakePaymentRequest>(), It.IsAny<Account>())).Returns(true);

            var sut = new PaymentService(factory.Object, accountValidatorBacs.Object, accountValidatorFasterPayments.Object, accountValidatorChaps.Object, appConfig.Object);
            var ret = sut.MakePayment(new MakePaymentRequest()
            {
                Amount = 100,
                DebtorAccountNumber = "12313-12-12",
                PaymentScheme = PaymentScheme.Bacs                
            });

            Assert.IsType<MakePaymentResult>(ret);
            Assert.True(true);
        }

    }
}
