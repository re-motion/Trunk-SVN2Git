// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// Provides a synchronization wrapper around an implementation of <see cref="ISecurityManagerPrincipal"/>.
  /// </summary>
  [Serializable]
  public class LockingSecurityManagerPrincipalDecorator : ISecurityManagerPrincipal
  {
    private readonly ISecurityManagerPrincipal _innerPrincipal;
    private readonly object _lock = new object();

    public LockingSecurityManagerPrincipalDecorator (ISecurityManagerPrincipal innerPrincipal)
    {
      ArgumentUtility.CheckNotNull ("innerPrincipal", innerPrincipal);

      _innerPrincipal = innerPrincipal;
    }

    bool INullObject.IsNull
    {
      get { return _innerPrincipal.IsNull; }
    }

    public TenantProxy Tenant
    {
      get
      {
        lock (_lock)
        {
          return _innerPrincipal.Tenant;
        }
      }
    }

    public UserProxy User
    {
      get
      {
        lock (_lock)
        {
          return _innerPrincipal.User;
        }
      }
    }

    public SubstitutionProxy Substitution
    {
      get
      {
        lock (_lock)
        {
          return _innerPrincipal.Substitution;
        }
      }
    }

    public void Refresh ()
    {
       lock (_lock)
       {
         _innerPrincipal.Refresh();
       }
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      lock (_lock)
      {
        return _innerPrincipal.GetSecurityPrincipal();
      }
    }

    public TenantProxy[] GetTenants (bool includeAbstractTenants)
    {
      lock (_lock)
      {
        return _innerPrincipal.GetTenants (includeAbstractTenants);
      }
    }

    public SubstitutionProxy[] GetActiveSubstitutions ()
    {
      lock (_lock)
      {
        return _innerPrincipal.GetActiveSubstitutions();
      }
    }
  }
}