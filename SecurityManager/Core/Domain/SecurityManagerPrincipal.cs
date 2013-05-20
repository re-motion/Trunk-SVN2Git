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
using System.Runtime.Serialization;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Security;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="SecurityManagerPrincipal"/> type represents the current <see cref="Tenant"/>, <see cref="User"/>, 
  /// and optional <see cref="Substitution"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The <see cref="Current"/> <see cref="SecurityManagerPrincipal"/> is hosted by the <see cref="SafeContext"/>, ie. it is thread-local in ordinary 
  /// applications and request-local (HttpContext) in applications using Remotion.Web.
  /// </para>
  /// <para>
  /// The domain objects held by a <see cref="SecurityManagerPrincipal"/> instance are stored in a dedicated <see cref="ClientTransaction"/>.
  /// Changes made to those objects are only saved when that transaction is committed, eg. via 
  /// <code>SecurityManagerPrincipal.Current.User.RootTransaction.Commit()</code>.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [Serializable]
  public sealed class SecurityManagerPrincipal : ISecurityManagerPrincipal
  {
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

    [NonSerialized]
    private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

    private int _revision;
    private readonly IDomainObjectHandle<Tenant> _tenantHandle;
    private readonly IDomainObjectHandle<User> _userHandle;
    private readonly IDomainObjectHandle<Substitution> _substitutionHandle;
    private TenantProxy _tenantProxy;
    private UserProxy _userProxy;
    private SubstitutionProxy _substitutionProxy;
    private ISecurityPrincipal _securityPrincipal;

    public SecurityManagerPrincipal (
        IDomainObjectHandle<Tenant> tenantHandle, IDomainObjectHandle<User> userHandle, IDomainObjectHandle<Substitution> substitutionHandle)
    {
      ArgumentUtility.CheckNotNull ("tenantHandle", tenantHandle);
      ArgumentUtility.CheckNotNull ("userHandle", userHandle);

      _tenantHandle = tenantHandle;
      _userHandle = userHandle;
      _substitutionHandle = substitutionHandle;

      InitializeCache (GetRevision());
    }

    public TenantProxy Tenant
    {
      get
      {
        _lock.EnterReadLock();
        try
        {
          return _tenantProxy;
        }
        finally
        {
          _lock.ExitReadLock();
        }
      }
    }

    public UserProxy User
    {
      get
      {
        _lock.EnterReadLock();
        try
        {
          return _userProxy;
        }
        finally
        {
          _lock.ExitReadLock();
        }
      }
    }

    public SubstitutionProxy Substitution
    {
      get
      {
        _lock.EnterReadLock();
        try
        {
          return _substitutionProxy;
        }
        finally
        {
          _lock.ExitReadLock();
        }
      }
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      _lock.EnterReadLock();
      try
      {
        return _securityPrincipal;
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    public void Refresh ()
    {
      _lock.EnterUpgradeableReadLock();
      try
      {
        var revision = GetRevision();
        if (revision != _revision)
          InitializeCache (revision);
      }
      finally
      {
        _lock.ExitUpgradeableReadLock();
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

    private void InitializeCache (int revision)
    {
      var transaction = CreateClientTransaction();

      var newTenantProxy = CreateTenantProxy (GetTenant (transaction));
      var newUserProxy = CreateUserProxy (GetUser (transaction));
      var substitution = GetSubstitution (transaction);
      var newSubstitutionProxy = substitution != null ? CreateSubstitutionProxy (substitution) : null;
      var newSecurityPrincipal = CreateSecurityPrincipal (transaction);

      _lock.EnterWriteLock();
      try
      {
        _revision = revision;
        _tenantProxy = newTenantProxy;
        _userProxy = newUserProxy;
        _substitutionProxy = newSubstitutionProxy;
        _securityPrincipal = newSecurityPrincipal;
      }
      finally
      {
        _lock.ExitWriteLock();
      }
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
      var transaction = ClientTransaction.CreateRootTransaction ();

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
        transaction.Extensions.Add (new SecurityClientTransactionExtension ());

      return transaction;
    }

    private int GetRevision ()
    {
      return (int) ClientTransaction.CreateRootTransaction().QueryManager.GetScalar (Revision.GetGetRevisionQuery());
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    [OnDeserialized]
    private void OnDeserialized (StreamingContext context)
    {
      _lock = new ReaderWriterLockSlim();
    }
  }
}