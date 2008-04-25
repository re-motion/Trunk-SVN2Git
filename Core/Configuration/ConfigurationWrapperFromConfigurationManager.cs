using System;
using System.Configuration;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  /// <summary>
  /// Concrete implementation of <see cref="ConfigurationWrapper"/> that uses the <see cref="ConfigurationManager"/>. Create the instance by
  /// invoking <see cref="ConfigurationWrapper.CreateFromConfigurationManager"/>.
  /// </summary>
  internal sealed class ConfigurationWrapperFromConfigurationManager: ConfigurationWrapper
  {
    public ConfigurationWrapperFromConfigurationManager()
    {
    }

    public override object GetSection (string sectionName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sectionName", sectionName);

      return ConfigurationManager.GetSection (sectionName);
    }

    public override ConnectionStringSettings GetConnectionString (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ConfigurationManager.ConnectionStrings[name];
    }

    public override string GetAppSetting (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ConfigurationManager.AppSettings[name];
    }
  }
}