// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Security.Configuration
{
  /// <summary>Helper class that loads implementations of <see cref="IPrincipalProvider"/> from the <see cref="SecurityConfiguration"/> section.</summary>
  public class PrincipalProviderHelper : ProviderHelperBase<IPrincipalProvider>
  {
    private const string c_threadPrincipalProviderWellKnownName = "Thread";
    private const string c_httpContexPrincipalProviderWellKnownName = "HttpContext";
    private const string c_securityManagerPrincipalProviderWellKnownName = "SecurityManager";

    private readonly object _sync = new object();
    private Type _httpContextPrincipalProviderType;
    private Type _securityManagerPrincipalProviderType;

    public PrincipalProviderHelper (SecurityConfiguration configuration)
        : base (configuration)
    {
    }

    protected override ConfigurationProperty CreateDefaultProviderNameProperty ()
    {
      return CreateDefaultProviderNameProperty ("defaultPrincipalProvider", c_threadPrincipalProviderWellKnownName);
    }

    protected override ConfigurationProperty CreateProviderSettingsProperty ()
    {
      return CreateProviderSettingsProperty ("principalProviders");
    }

    public override void PostDeserialze ()
    {
      CheckForDuplicateWellKownProviderName (c_threadPrincipalProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_httpContexPrincipalProviderWellKnownName);
      CheckForDuplicateWellKownProviderName (c_securityManagerPrincipalProviderWellKnownName);

      if (DefaultProviderName.Equals (c_httpContexPrincipalProviderWellKnownName, StringComparison.Ordinal))
        EnsureHttpContextPrincipalProviderTypeInitialized();

      if (DefaultProviderName.Equals (c_securityManagerPrincipalProviderWellKnownName, StringComparison.Ordinal))
        EnsureSecurityManagerPrincipalProviderTypeInitialized ();
    }

    protected override void EnsureWellKownProviders (ProviderCollection collection)
    {
      ArgumentUtility.CheckNotNull ("collection", collection);

      EnsureWellKnownThreadUserProvider (collection);
      EnsureWellKnownHttpContextPrincipalProvider (collection);
      EnsureWellKnownSecurityManagerPrincipalProvider (collection);
    }

    private void EnsureWellKnownThreadUserProvider (ProviderCollection collection)
    {
      collection.Add (new ThreadPrincipalProvider(c_threadPrincipalProviderWellKnownName, new NameValueCollection()));
    }

    private void EnsureWellKnownHttpContextPrincipalProvider (ProviderCollection collection)
    {
      if (_httpContextPrincipalProviderType != null)
      {
       collection.Add ((ExtendedProviderBase) Activator.CreateInstance (
          _httpContextPrincipalProviderType, 
          new object[] { c_httpContexPrincipalProviderWellKnownName, new NameValueCollection()}));
      }
    }

    private void EnsureHttpContextPrincipalProviderTypeInitialized ()
    {
      if (_httpContextPrincipalProviderType == null)
      {
        lock (_sync)
        {
          if (_httpContextPrincipalProviderType == null)
          {
            _httpContextPrincipalProviderType = GetTypeWithMatchingVersionNumber (
                DefaultProviderNameProperty,
                "Remotion.Web.Security",
                "Remotion.Web.Security.HttpContextPrincipalProvider");
          }
        }
      }
    }

    private void EnsureWellKnownSecurityManagerPrincipalProvider (ProviderCollection collection)
    {
      if (_securityManagerPrincipalProviderType != null)
      {
        collection.Add ((ExtendedProviderBase) Activator.CreateInstance (
            _securityManagerPrincipalProviderType,
            new object[] { c_securityManagerPrincipalProviderWellKnownName, new NameValueCollection () }));
      }
    }

    private void EnsureSecurityManagerPrincipalProviderTypeInitialized ()
    {
      if (_securityManagerPrincipalProviderType == null)
      {
        lock (_sync)
        {
          if (_securityManagerPrincipalProviderType == null)
          {
            _securityManagerPrincipalProviderType = GetType (
                DefaultProviderNameProperty,
                new AssemblyName ("Remotion.SecurityManager"),
                "Remotion.SecurityManager.Domain.SecurityManagerPrincipalProvider");
          }
        }
      }
    }
  }
}
