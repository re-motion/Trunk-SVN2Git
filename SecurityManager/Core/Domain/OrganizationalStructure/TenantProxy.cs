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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  public sealed class TenantProxy : BindableObjectWithIdentityBase
  {
    private readonly ObjectID _id;
    private readonly string _uniqueIdentifier;
    private readonly string _displayName;

    public static TenantProxy Create (Tenant tenant)
    {
      ArgumentUtility.CheckNotNull ("tenant", tenant);

      return new TenantProxy (tenant.ID, tenant.UniqueIdentifier, tenant.DisplayName);
    }

    private TenantProxy (ObjectID id, string uniqueIdentifier, string displayName)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      _id = id;
      _uniqueIdentifier = uniqueIdentifier;
      _displayName = displayName;
    }

    public ObjectID ID
    {
      get { return _id; }
    }

    public override string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public override string DisplayName
    {
      get { return _displayName; }
    }
  }
}