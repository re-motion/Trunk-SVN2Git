// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  /// <code>SecurityManagerPrincipal.Current.User.BindingTransaction.Commit()</code>.
  /// </para>
  /// </remarks>
  [Serializable]
  public class SecurityManagerPrincipal : ISecurityManagerPrincipal
  {
    private static readonly string s_currentKey = typeof (SecurityManagerPrincipal).AssemblyQualifiedName + "_Current";

    public static readonly ISecurityManagerPrincipal Null = new NullSecurityManagerPrincipal();

    public static ISecurityManagerPrincipal Current
    {
      get
      {
        return (ISecurityManagerPrincipal) SafeContext.Instance.GetData (s_currentKey) ?? SecurityManagerPrincipal.Null;
      }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        SafeContext.Instance.SetData (s_currentKey, value);
      }
    }

    private ClientTransaction _transaction;
    private readonly ObjectID _tenantID;
    private readonly ObjectID _userID;
    private readonly ObjectID _substitutionID;

    public SecurityManagerPrincipal (Tenant tenant, User user, Substitution substitution)
        : this (
            ArgumentUtility.CheckNotNull ("tenant", tenant).ID,
            ArgumentUtility.CheckNotNull ("user", user).ID,
            substitution != null ? substitution.ID : null)
    {
    }

    public SecurityManagerPrincipal (ObjectID tenantID, ObjectID userID, ObjectID substitutionID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);
      ArgumentUtility.CheckNotNull ("userID", userID);

      _tenantID = tenantID;
      _userID = userID;
      _substitutionID = substitutionID;

      InitializeClientTransaction();
    }

    public Tenant Tenant
    {
      get { return GetTenant (_transaction); }
    }

    public User User
    {
      get { return GetUser (_transaction); }
    }

    public Substitution Substitution
    {
      get { return GetSubstitution (_transaction); }
    }

    public void Refresh ()
    {
      InitializeClientTransaction ();
    }

    private Tenant GetTenant (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      using (transaction.EnterNonDiscardingScope())
      {
        return Tenant.GetObject (_tenantID);
      }
    }

    private User GetUser (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      using (transaction.EnterNonDiscardingScope())
      {
        return User.GetObject (_userID);
      }
    }

    private Substitution GetSubstitution (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      if (_substitutionID == null)
        return null;

      using (transaction.EnterNonDiscardingScope())
      {
        return (Substitution) Substitution.GetObject (_substitutionID);
      }
    }

    private void InitializeClientTransaction ()
    {
      _transaction = ClientTransaction.CreateBindingTransaction();
      //TODO: Test SecurityClientTransactionExtension
      //TODO: Has effect on Tenant.GetPossibleParentTenants and Tenant.GetHierarchy
      //if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
      //  _transaction.Extensions.Add (typeof (SecurityClientTransactionExtension).FullName, new SecurityClientTransactionExtension());
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      string substitutedUser = null;
      SecurityPrincipalRole substitutedRole = null;

      //TODO: Test SecurityFreeSection
      using (new SecurityFreeSection())
      {
        Substitution substitution = Substitution;
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

        return new SecurityPrincipal (User.UserName, null, substitutedUser, substitutedRole);
      }
    }
  }
}
