// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
