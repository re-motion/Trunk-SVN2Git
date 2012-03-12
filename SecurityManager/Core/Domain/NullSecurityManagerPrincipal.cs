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
using System.Runtime.Serialization;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="NullSecurityManagerPrincipal"/> type is the <see cref="INullObject"/> implementation 
  /// of the <see cref="ISecurityManagerPrincipal"/> interface.
  /// </summary>
  [Serializable]
  public sealed class NullSecurityManagerPrincipal:ISecurityManagerPrincipal, IObjectReference
  {
    internal NullSecurityManagerPrincipal ()
    {       
    }

    public TenantProxy Tenant
    {
      get { return null; }
    }

    public UserProxy User
    {
      get { return null; }
    }

    public SubstitutionProxy Substitution
    {
      get { return null; }
    }

    public void Refresh ()
    {
    }

    public TenantProxy[] GetTenants (bool includeAbstractTenants)
    {
      return new TenantProxy[0];
    }

    public SubstitutionProxy[] GetActiveSubstitutions ()
    {
      return new SubstitutionProxy[0];
    }

    public ISecurityPrincipal GetSecurityPrincipal ()
    {
      return new NullSecurityPrincipal();
    }

    bool INullObject.IsNull
    {
      get { return true; }
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return SecurityManagerPrincipal.Null;
    }
  }
}