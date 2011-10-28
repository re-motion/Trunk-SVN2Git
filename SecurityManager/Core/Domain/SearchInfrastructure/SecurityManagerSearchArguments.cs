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
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.SearchInfrastructure
{
  public class SecurityManagerSearchArguments : ITenantConstraint, IDisplayNameConstraint, IResultSizeConstraint
  {
    private readonly ObjectID _tenantID;
    private readonly string _displayNameFilter;
    private readonly int? _resultSize;

    public SecurityManagerSearchArguments (ObjectID tenantID, int? resultSize, string displayNameFilter)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);
      _tenantID = tenantID;
      _resultSize = resultSize;
      _displayNameFilter = displayNameFilter;
    }

    ObjectID ITenantConstraint.Value
    {
      get { return _tenantID; }
    }

    string IDisplayNameConstraint.Text
    {
      get { return _displayNameFilter; }
    }

    int? IResultSizeConstraint.Value
    {
      get { return _resultSize; }
    }
  }
}