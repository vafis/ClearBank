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
    public class Autofac
    {
        public static IContainer Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AccountValidatorBacs>()
                .As<IAccountValidator<AccountValidatorBacs>>().SingleInstance();
            builder.RegisterType<AccountValidatorFasterPayments>()
                .As<IAccountValidator<AccountValidatorFasterPayments>>().SingleInstance();
            builder.RegisterType<AccountValidatorChaps>()
                .As<IAccountValidator<AccountValidatorChaps>>().SingleInstance();
            builder.RegisterType<AppConfig>()
                .As<IAppConfig>().SingleInstance();
            builder.RegisterType<DataStoreFactory>()
                .As<IDataStoreFactory>().SingleInstance();


            var container = builder.Build();
            return container;

        }
    }
    public class Fixture : IDisposable
    {
        MakePaymentRequest _makePaymentRequest;
        Account _account;
        public Fixture()
        {
            AccountValidatorBacs = Autofac.Setup().Resolve<IAccountValidator<AccountValidatorBacs>>();
            AccountValidatorFasterPayments = Autofac.Setup().Resolve<IAccountValidator<AccountValidatorFasterPayments>>();
            AccountValidatorChaps = Autofac.Setup().Resolve<IAccountValidator<AccountValidatorChaps>>();
            AppConfig = Autofac.Setup().Resolve<IAppConfig>();
            DataStoreFactory = Autofac.Setup().Resolve<IDataStoreFactory>();

            _makePaymentRequest = new MakePaymentRequest();
            _account = new Account()
            {
                AccountNumber = "1111-2222-33333",
                Status = AccountStatus.Live,
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps | AllowedPaymentSchemes.FasterPayments
            };

        }
        public IAccountValidator<AccountValidatorBacs> AccountValidatorBacs { get; private set; }
        public IAccountValidator<AccountValidatorFasterPayments> AccountValidatorFasterPayments { get; private set; }
        public IAccountValidator<AccountValidatorChaps> AccountValidatorChaps { get; private set; }
        public IAppConfig AppConfig { get; private set; }
        public IDataStoreFactory DataStoreFactory { get; private set; }
        public MakePaymentRequest MakePaymentRequest { get
            {
                return _makePaymentRequest;
            } }
        public Account Account
        {
            get
            {
                return _account;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class IntegrationTests : IClassFixture<Fixture>
    {
        private Fixture _fixture;
        public IntegrationTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void PaymentService_Tests()
        {
            var paymentService = new PaymentService(_fixture.DataStoreFactory,
                                                   _fixture.AccountValidatorBacs,
                                                   _fixture.AccountValidatorFasterPayments,
                                                   _fixture.AccountValidatorChaps,
                                                   _fixture.AppConfig);
            var ret = paymentService.MakePayment(_fixture.MakePaymentRequest);

            Assert.False(ret.Success);
            Assert.IsType<MakePaymentResult>(ret);
        }
    }
  
}
