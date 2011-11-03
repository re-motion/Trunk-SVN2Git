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
using System.Linq;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.SearchInfrastructure;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Implementation of <see cref="ISearchAvailableObjectsService"/> for the <see cref="AccessControlEntry"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="AccessControlEntry.SpecificPosition"/>, <see cref="AccessControlEntry.SpecificGroupType"/>
  /// and <see cref="AccessControlEntry.SpecificAbstractRole"/> properties via the <see cref="SearchAvailableObjectsServiceTypeAttribute"/>.
  /// </remarks>
  public class AccessControlEntryPropertiesSearchService : SecurityManagerPropertyBasedSearchServiceBase<AccessControlEntry>
  {
    public AccessControlEntryPropertiesSearchService ()
    {
      RegisterQueryFactory ("SpecificAbstractRole", delegate { return AbstractRoleDefinition.FindAll().Cast<IBusinessObject>().AsQueryable(); });
    }
  }
}