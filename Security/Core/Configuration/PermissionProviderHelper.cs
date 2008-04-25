using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using Remotion.Configuration;
using Remotion.Security.Metadata;
using Remotion.Utilities;

namespace Remotion.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="IPermissionProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class PermissionProviderHelper : ProviderHelperBase<IPermissionProvider>
  {
    private const string c_permissionReflectorWellKnownName = "Reflection";

    public PermissionProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultPermissionProvider", c_permissionReflectorWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("permissionProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_permissionReflectorWellKnownName);
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownReflectionPermissionProvider (collection);
    }

    private void EnsureWellKnownReflectionPermissionProvider (ProviderCollection collection)
    {
      collection.Add (new PermissionReflector(c_permissionReflectorWellKnownName, new NameValueCollection()));
    }
  }
}