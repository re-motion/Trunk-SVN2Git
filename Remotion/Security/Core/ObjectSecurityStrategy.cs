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
using System.Linq;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Security
{
  /// <summary>
  /// Default implementation of the <see cref="IObjectSecurityStrategy"/> interface. A new instance of the <see cref="ObjectSecurityStrategy"/> type
  /// is typically created and held for each <see cref="ISecurableObject"/> implementation.
  /// </summary>
  /// <remarks>
  /// The <see cref="ObjectSecurityStrategy"/> supports the use of an <see cref="ISecurityContextFactory"/> for creating the relevant <see cref="ISecurityContext"/>, 
  /// an <see cref="IAccessTypeFilter"/> for filtering the allowed access types returned by the <see cref="ISecurityProvider"/>, 
  /// and caches the result.
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

    //TODO RM-6183: Refactor AccessType[] to IReadOnlyList<AccessType> and implement a Singleton-Version to allow for non-allocating checks

    private readonly ICache<ISecurityPrincipal, AccessType[]> _cache;
    private readonly ISecurityContextFactory _securityContextFactory;
    private readonly IAccessTypeFilter _accessTypeFilter;
    private readonly CacheInvalidationToken _cacheInvalidationToken;

    public ObjectSecurityStrategy (
        ISecurityContextFactory securityContextFactory,
        IAccessTypeFilter accessTypeFilter,
        CacheInvalidationToken cacheInvalidationToken)
    {
      ArgumentUtility.CheckNotNull ("securityContextFactory", securityContextFactory);
      ArgumentUtility.CheckNotNull ("accessTypeFilter", accessTypeFilter);
      ArgumentUtility.CheckNotNull ("cacheInvalidationToken", cacheInvalidationToken);

      _securityContextFactory = securityContextFactory;
      _accessTypeFilter = accessTypeFilter;
      _cacheInvalidationToken = cacheInvalidationToken;
      _cache = CacheFactory.Create<ISecurityPrincipal, AccessType[]> (cacheInvalidationToken);
    }

    public ISecurityContextFactory SecurityContextFactory
    {
      get { return _securityContextFactory; }
    }

    public CacheInvalidationToken CacheInvalidationToken
    {
      get { return _cacheInvalidationToken; }
    }

    public bool HasAccess (ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.DebugCheckNotNull ("securityProvider", securityProvider);
      ArgumentUtility.DebugCheckNotNull ("principal", principal);
      ArgumentUtility.CheckNotNull ("requiredAccessTypes", requiredAccessTypes);
      // Performance critical argument check. Can be refactored to ArgumentUtility.CheckNotNullOrEmpty once typed collection checks are supported.
      if (requiredAccessTypes.Length == 0)
        throw ArgumentUtility.CreateArgumentEmptyException ("requiredAccessTypes");

      var actualAccessTypes = GetAccessTypesFromCache (securityProvider, principal);
      return requiredAccessTypes.IsSubsetOf (actualAccessTypes);
    }

    private AccessType[] GetAccessTypesFromCache (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      AccessType[] value;

      if (!_cache.TryGetValue (principal, out value))
        value = _cache.GetOrCreateValue (principal, delegate { return GetAccessTypes (securityProvider, principal); });

      return value;
    }

    private AccessType[] GetAccessTypes (ISecurityProvider securityProvider, ISecurityPrincipal principal)
    {
      // Explicit null-check since the public method does not perform this check in release-code
      ArgumentUtility.CheckNotNull ("securityProvider", securityProvider);

      var context = CreateSecurityContext();

      var accessTypes = securityProvider.GetAccess (context, principal);
      Assertion.IsNotNull (accessTypes, "GetAccess evaluated and returned null.");

      return FilterAccessTypes (accessTypes, principal, context);
    }

    private ISecurityContext CreateSecurityContext ()
    {
      using (new SecurityFreeSection())
      {
        var context = _securityContextFactory.CreateSecurityContext();
        Assertion.IsNotNull (context, "ISecurityContextFactory.CreateSecurityContext() evaluated and returned null.");

        return context;
      }
    }

    private AccessType[] FilterAccessTypes (AccessType[] accessTypes, ISecurityPrincipal principal, ISecurityContext context)
    {
      var filteredAccessTypes = _accessTypeFilter.Filter (accessTypes, context, principal).ToArray();

      if (!filteredAccessTypes.IsSubsetOf (accessTypes))
      {
        throw new InvalidOperationException (
            string.Format (
                "The access type filter injected additional access types ('{0}') into the filter result. "
                + "An access type filter may only remove (i.e. filter) the list of access types returned from the security provider.",
                string.Join ("', '", filteredAccessTypes.Except (accessTypes))));
      }

      return filteredAccessTypes;
    }
  }
}