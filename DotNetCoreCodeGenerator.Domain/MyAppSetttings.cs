using DotNetCodeGenerator.Domain.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using HelpersProject;

namespace DotNetCoreCodeGenerator.Domain
{
    public class MyAppSetttings
    {
        public const string ConnectionStringKey = "DefaultConnection";
        private IConfiguration Configuration { get; set; }
        private ILogger Logger { get; set; }
        public MyAppSetttings(IConfiguration config, ILogger<MyAppSetttings> logger)
        {
            Configuration = config;
            Logger = logger;
        }
        public String MyConfig 
        {
            get
            {
                return Configuration["MyConfig"].ToStr();
            }
        }
       public int CacheMediumSeconds
        {
            get
            {
                return GetConfigInt("CacheMediumSeconds", 1000);
            }
        }
       
        public string GetConfigString(string configName, string defaultValue = "")
        {
            var appValue = Configuration[configName];
            if (String.IsNullOrEmpty(appValue))
            {
                Logger.LogInformation(String.Format("Config Name {0} is using default value {1}      <add key=\"{0}\" value=\"{1}\" />", configName, defaultValue));
                return defaultValue;
            }
            else
            {
                return appValue;
            }
        }

        public bool GetConfigBool(string configName, bool defaultValue = false)
        {
            //return !String.IsNullOrEmpty(WebConfigurationManager.AppSettings[configName]) ? WebConfigurationManager.AppSettings[configName].ToBool() : defaultValue;

            var configValue = defaultValue;
            var appValue = Configuration[configName];
            if (!String.IsNullOrEmpty(appValue))
            {
                configValue = appValue.ToBool();
            }
            else
            {
                Logger.LogInformation(String.Format("Config Name {0} is using default value {1}  <add key=\"{0}\" value=\"{1}\" />", configName, defaultValue));
            }
            return configValue;

        }

        public int GetConfigInt(string configName, int defaultValue = 0)
        {
            int configValue = -1;
            var appValue = Configuration[configName];
            if (!String.IsNullOrEmpty(appValue))
            {
                configValue = appValue.ToInt();
            }
            else
            {
                Logger.LogInformation(String.Format("Config Name {0} is using default value {1}   <add key=\"{0}\" value=\"{1}\" />", configName, defaultValue));
            }
            return configValue == -1 ? defaultValue : configValue;
        }
    }
}
