// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
  /// <summary>Helper class that loads implementations of <see cref="IUserProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class UserProviderHelper : ProviderHelperBase<IUserProvider>
  {
    private const string c_threadUserProviderWellKnownName = "Thread";
    private const string c_httpContexUserProviderWellKnownName = "HttpContext";

    private readonly object _sync = new object();
    private Type _httpContextUserProviderType;

    public UserProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultUserProvider", c_threadUserProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("userProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_threadUserProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_httpContexUserProviderWellKnownName);

      if (DefaultProviderName.Equals (c_httpContexUserProviderWellKnownName, StringComparison.Ordinal))
        EnsureHttpContextUserProviderTypeInitialized();
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownThreadUserProvider (collection);
      EnsureWellKnownHttpContextUserProvider (collection);
    }

    private void EnsureWellKnownThreadUserProvider (ProviderCollection collection)
    {
      collection.Add (new ThreadUserProvider(c_threadUserProviderWellKnownName, new NameValueCollection()));
    }

    private void EnsureWellKnownHttpContextUserProvider (ProviderCollection collection)
    {
      if (_httpContextUserProviderType != null)
      {
       collection.Add ((ExtendedProviderBase) Activator.CreateInstance (
          _httpContextUserProviderType, 
          new object[] { c_httpContexUserProviderWellKnownName, new NameValueCollection()}));
      }
    }

    private void EnsureHttpContextUserProviderTypeInitialized ()
    {
      if (_httpContextUserProviderType == null)
      {
        lock (_sync)
        {
          if (_httpContextUserProviderType == null)
          {
            _httpContextUserProviderType = GetTypeWithMatchingVersionNumber (
                DefaultProviderNameProperty,
                "Remotion.Web.Security",
                "Remotion.Web.Security.HttpContextUserProvider");
          }
        }
      }
    }
  }
}
