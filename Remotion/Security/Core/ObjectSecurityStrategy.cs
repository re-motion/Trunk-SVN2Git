// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Default implementation of the <see cref="IObjectSecurityStrategy"/> interface. A new instance of the <see cref="ObjectSecurityStrategy"/> type
  /// is typically created and held for each <see cref="ISecurableObject"/> implementation.
  /// </summary>
  /// <remarks>
  /// The <see cref="ObjectSecurityStrategy"/> supports the use of an <see cref="ISecurityContextFactory"/> 
  /// for creating the relevant <see cref="ISecurityContext"/> and caches the result returned by the <see cref="ISecurityProvider"/>.
  /// </remarks>
  /// <threadsafety static="true" instance="false" />
  [Serializable]
  public sealed class ObjectSecurityStrategy : IObjectSecurityStrategy
  {
    #region Obsolete

    [Obsolete ("Use CacheInvalidationToken.Invalidate() instead. (Version 1.15.20.0)", true)]
    public void InvalidateLocalCache ()
    {
      throw new NotImplementedException ("Use CacheInvalidationToken.Invalidate() instead. (Version 1.15.20.0)");
    }

    [Obsolete (
        "Use new ObjectSecurityStrategy (securityContextFactory, NullAccessTypeFilter.Null, new CacheInvalidationToken()) instead. (Version 1.15.20.0)",
        true)]
    private ObjectSecurityStrategy (ISecurityContextFactory securityContextFactory)
    {
      throw new NotImplementedException (
          "Use new ObjectSecurityStrategy (securityContextFactory, NullAccessTypeFilter.Null, new CacheInvalidationToken()) instead. (Version 1.15.20.0)");
    }

    #endregion

    private readonly ICache<ISecurityPrincipal, AccessType[]> _cache;
    private readonly ISecurityContextFactory _securityContextFactory;

    public ObjectSecurityStrategy (
        ISecurityContextFactory securityContextFactory,
        InvalidationToken invalidationToken)
    {
      ArgumentUtility.CheckNotNull ("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull ("invalidationToken", invalidationToken);

      _securityContextFactory = securityContextFactory;
      _cache = CacheFactory.Create<ISecurityPrincipal, AccessType[]> (invalidationToken);
    }

    public bool HasAccess (ISecurityProvider securityProvider, ISecurityPrincipal principal, IReadOnlyList<AccessType> requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);
      // Performance critical argument check. Can be refactored to ArgumentUtility.CheckNotNullOrEmpty once typed collection checks are supported.
      if (requiredAccessTypes.Count == 0)
        throw ArgumentUtility.CreateArgumentEmptyException ("requiredAccessTypes");

      var actualAccessTypes = GetAccessTypesFromCache (securityProvider, principal);
      return requiredAccessTypes.IsSubsetOf (actualAccessTypes);
    }

    private AccessType[] GetAccessTypesFromCache (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      AccessType[] value;
      if (_cache.TryGetValue (principal, out value))
        return value;

      // Split to prevent closure being created during the TryGetValue-operation
      return GetOrCreateAccessTypesFromCache (securityProvider, principal);
    }

    private AccessType[] GetOrCreateAccessTypesFromCache (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      return _cache.GetOrCreateValue (principal, key => GetAccessTypes (securityProvider, key));
    }

    private AccessType[] GetAccessTypes (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      // Explicit null-check since the public method does not perform this check in release-code
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);

      var context = CreateSecurityContext();

      var accessTypes = securityProvider.GetAccess (context, principal);
      Assertion.IsNotNull (accessTypes, "GetAccess evaluated and returned null.");

      return accessTypes;
    }

    private ISecurityContext CreateSecurityContext ()
    {
      using (SecurityFreeSection.Activate())
      {
        var context = _securityContextFactory.CreateSecurityContext();
        Assertion.IsNotNull (context, "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

        return context;
      }
    }
  }
}