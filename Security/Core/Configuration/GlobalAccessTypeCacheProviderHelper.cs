/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
