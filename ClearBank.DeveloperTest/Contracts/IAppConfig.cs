using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearBank.DeveloperTest.Contracts
{
    public interface IAppConfig
    {
        string GetKeyValue(string key);
    }
}
