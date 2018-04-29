using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Data;


namespace ClearBank.DeveloperTest.Contracts
{
    public interface IDataStoreFactory
    {
         IDataStore Create(string dataStoreType);
    }
}
