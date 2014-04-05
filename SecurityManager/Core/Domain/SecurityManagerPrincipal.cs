// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="SecurityManagerPrincipal"/> type represents the current <see cref="Tenant"/>, <see cref="User"/>, 
  /// and optional <see cref="Substitution"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="Current"/> <see cref="SecurityManagerPrincipal"/> is hosted by the <see cref="SafeContext"/>, i.e. it is thread-local in ordinary 
  /// applications and request-local (HttpContext) in applications using Remotion.Web.
  /// </para>
  /// <para>
  /// The domain objects held by a <see cref="SecurityManagerPrincipal"/> instance are stored in a dedicated <see cref="ClientTransaction"/>.
  /// Changes made to those objects are only saved when that transaction is committed, eg. via 
  /// <code>SecurityManagerPrincipal.Current.User.RootTransaction.Commit()</code>.
  /// </para>
  /// <para>
  /// Refreshing the <see cref="SecurityManagerPrincipal"/> via the <see cref="Refresh"/> method must be performed 
  /// on the same thread where the refreshed data is required. Otherwise the result could be stale due to memory optimizations.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [Serializable]
  public sealed class SecurityManagerPrincipal : ISecurityManagerPrincipal
  {
    [Serializable]
    private sealed class Data
    {
      public readonly GuidRevisionValue Revision;
      public readonly TenantProxy TenantProxy;
      public readonly UserProxy UserProxy;
      public readonly SubstitutionProxy SubstitutionProxy;
      public readonly ISecurityPrincipal SecurityPrincipal;

      public Data (
          GuidRevisionValue revision,
          TenantProxy tenantProxy,
          UserProxy userProxy,
          SubstitutionProxy substitutionProxy,
          ISecurityPrincipal securityPrincipal)
      {
        Revision = revision;
        TenantProxy = tenantProxy;
        UserProxy = userProxy;
        SubstitutionProxy = substitutionProxy;
        SecurityPrincipal = securityPrincipal;
      }
    }

    public static readonly ISecurityManagerPrincipal Null = new NullSecurityManagerPrincipal();

    [NotNull]
    public static ISecurityManagerPrincipal Current
    {
      get { return (ISecurityManagerPrincipal) SafeContext.Instance.GetData (SafeContextKeys.SecurityManagerPrincipalCurrent) ?? Null; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        SafeContext.Instance.SetData (SafeContextKeys.SecurityManagerPrincipalCurrent, value);
      }
    }

    private readonly object _syncRoot;
    private Data _cachedData;
    private readonly IDomainObjectHandle<Tenant> _tenantHandle;
    private readonly IDomainObjectHandle<User> _userHandle;
    private readonly IDomainObjectHandle<Substitution> _substitutionHandle;

    public SecurityManagerPrincipal (
        IDomainObjectHandle<Tenant> tenantHandle, IDomainObjectHandle<User> userHandle, IDomainObjectHandle<Substitution> substitutionHandle)
    {
      ArgumentUtility.CheckNotNull ("tenantHandle", tenantHandle);
      ArgumentUtility.CheckNotNull ("userHandle", userHandle);

      _syncRoot = new object();

      _tenantHandle = tenantHandle;
      _userHandle = userHandle;
      _substitutionHandle = substitutionHandle;

      _cachedData = CreateDataObject (GetRevision());
    }

    public TenantProxy Tenant
    {
      get { return GetCachedDataVolatile().TenantProxy; }
    }

    public UserProxy User
    {
      get { return GetCachedDataVolatile().UserProxy; }
    }

    public SubstitutionProxy Substitution
    {
      get { return GetCachedDataVolatile().SubstitutionProxy; }
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      return GetCachedDataVolatile().SecurityPrincipal;
    }

    public void Refresh ()
    {
      var currentRevision = GetRevision();
      lock (_syncRoot)
      {
        if (!_cachedData.Revision.IsCurrent (currentRevision))
          _cachedData = CreateDataObject (currentRevision);
      }
    }

    public TenantProxy[] GetTenants (bool includeAbstractTenants)
    {
      Tenant tenant;
      using (new SecurityFreeSection())
      {
        tenant = GetUser (CreateClientTransaction()).Tenant;
      }

      return tenant.GetHierachy()
                   .Where (t => includeAbstractTenants || !t.IsAbstract)
                   .Select (CreateTenantProxy)
                   .ToArray();
    }

    public SubstitutionProxy[] GetActiveSubstitutions ()
    {
      return GetUser (CreateClientTransaction()).GetActiveSubstitutions()
                                                .Select (CreateSubstitutionProxy)
                                                .ToArray();
    }

    private SecurityPrincipal CreateSecurityPrincipal (ClientTransaction transaction)
    {
      using (new SecurityFreeSection())
      {
        string user = GetUser (transaction).UserName;
        ISecurityPrincipalRole role = null;

        string substitutedUser = null;
        ISecurityPrincipalRole substitutedRole = null;

        Substitution substitution = GetSubstitution (transaction);
        if (substitution != null)
        {
          substitutedUser = substitution.SubstitutedUser.UserName;
          if (substitution.SubstitutedRole != null)
          {
            substitutedRole = new SecurityPrincipalRole (
                substitution.SubstitutedRole.Group.UniqueIdentifier,
                substitution.SubstitutedRole.Position.UniqueIdentifier);
          }
        }

        return new SecurityPrincipal (user, role, substitutedUser, substitutedRole);
      }
    }

    private TenantProxy CreateTenantProxy (Tenant tenant)
    {
      using (new SecurityFreeSection())
      {
        return TenantProxy.Create (tenant);
      }
    }

    private UserProxy CreateUserProxy (User user)
    {
      using (new SecurityFreeSection())
      {
        return UserProxy.Create (user);
      }
    }

    private SubstitutionProxy CreateSubstitutionProxy (Substitution substitution)
    {
      using (new SecurityFreeSection())
      {
        return SubstitutionProxy.Create (substitution);
      }
    }

    private Data CreateDataObject (GuidRevisionValue revision)
    {
      var transaction = CreateClientTransaction();

      var tenantProxy = CreateTenantProxy (GetTenant (transaction));
      var userProxy = CreateUserProxy (GetUser (transaction));
      var substitution = GetSubstitution (transaction);
      var substitutionProxy = substitution != null ? CreateSubstitutionProxy (substitution) : null;
      var securityPrincipal = CreateSecurityPrincipal (transaction);

      return new Data (revision, tenantProxy, userProxy, substitutionProxy, securityPrincipal);
    }

    private Tenant GetTenant (ClientTransaction transaction)
    {
      return _tenantHandle.GetObject (transaction);
    }

    private User GetUser (ClientTransaction transaction)
    {
      return _userHandle.GetObject (transaction);
    }

    private Substitution GetSubstitution (ClientTransaction transaction)
    {
      if (_substitutionHandle == null)
        return null;

      return (Substitution) LifetimeService.GetObject (transaction, _substitutionHandle.ObjectID, false);
    }

    private ClientTransaction CreateClientTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
        transaction.Extensions.Add (new SecurityClientTransactionExtension());

      return transaction;
    }

    /// <summary>
    /// Potentially moving the read-access to field _cachedData to an earlier point in time within the current thread is not an issue:
    /// If a call to Refresh() on Thread #2 would cause a reload of the data, 
    /// and the read-access of _cachedData on Thread #1 has been optimized to an ealier point in time, 
    /// the result would only be stale during this one request.
    /// ==> Synchronized refresh accross all threads is a non-goal for the SecurityManagerPrincipal.
    /// </summary>
    private Data GetCachedDataVolatile ()
    {
      return Volatile.Read (ref _cachedData);
    }

    private GuidRevisionValue GetRevision ()
    {
      return SafeServiceLocator.Current.GetInstance<IDomainRevisionProvider>().GetRevision(new RevisionKey());
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}