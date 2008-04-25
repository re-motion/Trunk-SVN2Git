using System;
using System.Configuration;
using System.Configuration.Provider;
using Remotion.Configuration;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class StorageProviderDefinitionHelper: ProviderHelperBase<StorageProviderDefinition>
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing


    public StorageProviderDefinitionHelper (ExtendedConfigurationSection configurationSection)
        : base (configurationSection)
    {
    }

    // methods and properties

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultProviderDefinition", null);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("providerDefinitions");
    }
  }
}