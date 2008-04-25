using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="IGlobalAccessTypeCacheProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class GlobalAccessTypeCacheProviderHelper : ProviderHelperBase<IGlobalAccessTypeCacheProvider>
  {
    private const string c_nullGlobalAccessTypeCacheProviderWellKnownName = "None";
    private const string c_revisionBasedGlobalAccessTypeCacheProviderWellKnownName = "RevisionBased";

    public GlobalAccessTypeCacheProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultGlobalAccessTypeCacheProvider", c_nullGlobalAccessTypeCacheProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("globalAccessTypeCacheProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_nullGlobalAccessTypeCacheProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_revisionBasedGlobalAccessTypeCacheProviderWellKnownName);
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownNullGlobalAccessTypeCacheProvider (collection);
      EnsureWellKnownRevisionBasedGlobalAccessTypeCacheProvider (collection);
    }

    private void EnsureWellKnownNullGlobalAccessTypeCacheProvider (ProviderCollection collection)
    {
      collection.Add (new NullGlobalAccessTypeCacheProvider (c_nullGlobalAccessTypeCacheProviderWellKnownName, new NameValueCollection()));
    }

    private void EnsureWellKnownRevisionBasedGlobalAccessTypeCacheProvider (ProviderCollection collection)
    {
      collection.Add (new RevisionBasedAccessTypeCacheProvider (c_revisionBasedGlobalAccessTypeCacheProviderWellKnownName, new NameValueCollection()));
    }
  }
}