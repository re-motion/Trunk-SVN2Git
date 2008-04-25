using System;
using System.Configuration;
using Remotion.Configuration;

namespace Remotion.UnitTests.Configuration
{
  public class StubExtendedConfigurationSection : ExtendedConfigurationSection
  {
    // constants

    // types

    // static members

    // member fields

    private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private StubProviderHelper _stubProviderHelper;

    // construction and disposing

    public StubExtendedConfigurationSection (
        string wellKnownProviderID,
        string defaultProviderName,
        string defaultProviderID,
        string providerCollectionName
        )
    {
      _stubProviderHelper = new StubProviderHelper (this, wellKnownProviderID, defaultProviderName, defaultProviderID, providerCollectionName);
      _stubProviderHelper.InitializeProperties (_properties);
    }

    // methods and properties

    public StubProviderHelper GetStubProviderHelper ()
    {
      return _stubProviderHelper;
    }

    public ConfigurationPropertyCollection GetProperties ()
    {
      return _properties;
    }

    protected override void PostDeserialize ()
    {
      base.PostDeserialize();

      _stubProviderHelper.PostDeserialze();
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }
  }
}