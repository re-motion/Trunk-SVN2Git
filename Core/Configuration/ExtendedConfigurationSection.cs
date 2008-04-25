using System;
using System.Configuration;

namespace Remotion.Configuration
{
  /// <summary>Base class for all configuration sections using the <see cref="ProviderHelperBase"/> to manage their provider sections.</summary>
  public abstract class ExtendedConfigurationSection: ConfigurationSection
  {
    protected ExtendedConfigurationSection()
    {
    }

    protected internal new object this [ConfigurationProperty property]
    {
      get { return base[property]; }
      set { base[property] = value; }
    }
  }
}