using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ClearBank.DeveloperTest.Contracts;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    public class DataStoreFactory: IDataStoreFactory
    {

        public IDataStore Create(string dataStoreType)
        {
            IDataStore datastore = null;
            if (dataStoreType == "Backup")
            {
                datastore = new BackupAccountDataStore();               
            }
            else
            {
                datastore = new AccountDataStore();               
            }
            return datastore;
        }
    }
}
