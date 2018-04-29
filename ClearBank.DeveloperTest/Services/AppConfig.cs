using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClearBank.DeveloperTest.Contracts;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class AppConfig : IAppConfig
    {
        public string GetKeyValue(string key)
        {
           return ConfigurationManager.AppSettings[key];
        }
    }
}
