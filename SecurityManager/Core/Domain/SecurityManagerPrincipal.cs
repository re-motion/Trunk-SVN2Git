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
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
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
  /// The domain objects held by a <see cref="SecurityManagerPrincipal"/> instance are stored in a dedicated <see cref="BindingClientTransaction"/>.
  /// Changes made to those objects are only saved when that transaction is committed, eg. via 
  /// <code>SecurityManagerPrincipal.Current.User.BindingTransaction.CommitAllEndPoints()</code>.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="false"/>
  [Serializable]
  public class SecurityManagerPrincipal : ISecurityManagerPrincipal
  {
    private static readonly string s_currentKey = typeof (SecurityManagerPrincipal).AssemblyQualifiedName + "_Current";

    public static readonly ISecurityManagerPrincipal Null = new NullSecurityManagerPrincipal();

    public static ISecurityManagerPrincipal Current
    {
      // TODO 4650: When SafeContext is moved to 'Common' remove reference to Mixins.Core
      get { return (ISecurityManagerPrincipal) SafeContext.Instance.GetData (s_currentKey) ?? Null; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        SafeContext.Instance.SetData (s_currentKey, value);
      }
    }

    private ClientTransaction _transaction;
    private int _revision;
    private readonly ObjectID _tenantID;
    private readonly ObjectID _userID;
    private readonly ObjectID _substitutionID;

    [NonSerialized]
    private ISecurityPrincipal _securityPrincipal;

    [NonSerialized]
    private TenantProxy _tenantProxy;

    [NonSerialized]
    private UserProxy _userProxy;

    [NonSerialized]
    private SubstitutionProxy _substitutionProxy;

    public SecurityManagerPrincipal (ObjectID tenantID, ObjectID userID, ObjectID substitutionID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);
      ArgumentUtility.CheckNotNull ("userID", userID);
      ArgumentUtility.CheckTypeIsAssignableFrom ("tenantID", tenantID.ClassDefinition.ClassType, typeof (Tenant));
      ArgumentUtility.CheckTypeIsAssignableFrom ("userID", userID.ClassDefinition.ClassType, typeof (User));
      if (substitutionID != null)
        ArgumentUtility.CheckTypeIsAssignableFrom ("substitutionID", substitutionID.ClassDefinition.ClassType, typeof (Substitution));

      _tenantID = tenantID;
      _userID = userID;
      _substitutionID = substitutionID;

      InitializeClientTransaction();
    }

    public TenantProxy Tenant
    {
      get
      {
        if (_tenantProxy == null)
          _tenantProxy = CreateTenantProxy (GetTenant (_transaction));
        return _tenantProxy;
      }
    }

    public UserProxy User
    {
      get
      {
        if (_userProxy == null)
          _userProxy = CreateUserProxy (GetUser (_transaction));
        return _userProxy;
      }
    }

    public SubstitutionProxy Substitution
    {
      get
      {
        if (_substitutionProxy == null)
        {
          Substitution substitution = GetSubstitution (_transaction);
          _substitutionProxy = substitution != null ? CreateSubstitutionProxy (substitution) : null;
        }
        return _substitutionProxy;
      }
    }

    public void Refresh ()
    {
      if (GetRevision() > _revision)
      {
        ResetCache();
        InitializeClientTransaction();
      }
    }

    public TenantProxy[] GetTenants (bool includeAbstractTenants)
    {
      Tenant tenant;
      using (new SecurityFreeSection())
      {
        tenant = GetUser (_transaction).Tenant;
      }

      return tenant.GetHierachy()
          .Where (t => includeAbstractTenants || !t.IsAbstract)
          .Select (CreateTenantProxy)
          .ToArray();
    }

    public SubstitutionProxy[] GetActiveSubstitutions ()
    {
      return GetUser (_transaction).GetActiveSubstitutions()
          .Select (CreateSubstitutionProxy)
          .ToArray();
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      if (_securityPrincipal == null)
        _securityPrincipal = CreateSecurityPrincipal();
      return _securityPrincipal;
    }

    private SecurityPrincipal CreateSecurityPrincipal ()
    {
      using (new SecurityFreeSection())
      {
        string user = GetUser (_transaction).UserName;
        ISecurityPrincipalRole role = null;

        string substitutedUser = null;
        ISecurityPrincipalRole substitutedRole = null;

        Substitution substitution = GetSubstitution (_transaction);
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

    private void ResetCache ()
    {
      _tenantProxy = null;
      _userProxy = null;
      _substitutionProxy = null;
      _securityPrincipal = null;
    }

    private Tenant GetTenant (ClientTransaction transaction)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        return OrganizationalStructure.Tenant.GetObject (_tenantID);
      }
    }

    private User GetUser (ClientTransaction transaction)
    {
      using (transaction.EnterNonDiscardingScope ())
      {
        return OrganizationalStructure.User.GetObject (_userID);
      }
    }

    private Substitution GetSubstitution (ClientTransaction transaction)
    {
      if (_substitutionID == null)
        return null;

      using (transaction.EnterNonDiscardingScope ())
      {
        return (Substitution) OrganizationalStructure.Substitution.GetObject (_substitutionID);
      }
    }

    private void InitializeClientTransaction ()
    {
      _transaction = ClientTransaction.CreateBindingTransaction ();
      _revision = GetRevision ();

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
        _transaction.Extensions.Add (new SecurityClientTransactionExtension ());
    }

    private int GetRevision ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        return Revision.GetRevision();
      }
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}