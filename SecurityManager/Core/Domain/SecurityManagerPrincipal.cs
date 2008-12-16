// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="SecurityManagerPrincipal"/> type is represent the current <see cref="Tenant"/>, <see cref="User"/>, 
  /// and optional <see cref="Substitution"/>.
  /// </summary>
  public class SecurityManagerPrincipal
  {
    private static readonly string s_currentKey = typeof (SecurityManagerPrincipal).AssemblyQualifiedName + "_Current";

    public static SecurityManagerPrincipal Current
    {
      get { return (SecurityManagerPrincipal) SafeContext.Instance.GetData (s_currentKey); }
      set { SafeContext.Instance.SetData (s_currentKey, value); }
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

    public Tenant GetTenant (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      using (transaction.EnterNonDiscardingScope())
      {
        return Tenant.GetObject (_tenantID);
      }
    }

    public User GetUser (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      using (transaction.EnterNonDiscardingScope())
      {
        return User.GetObject (_userID);
      }
    }

    public Substitution GetSubstitution (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      if (_substitutionID == null)
        return null;

      using (transaction.EnterNonDiscardingScope())
      {
        return (Substitution) Substitution.GetObject (_substitutionID);
      }
    }

    public void Refresh ()
    {
      InitializeClientTransaction();
    }

    private void InitializeClientTransaction ()
    {
      _transaction = ClientTransaction.CreateBindingTransaction();
    }
  }
}