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
using Remotion.Data.DomainObjects.Linq;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  /// <summary>
  /// Defines query extensions for <see cref="SecurableClassDefinition"/>
  /// </summary>
  public static class SecurableClassDefinitionExtensions
  {
    public static IQueryable<SecurableClassDefinition> FetchDetails (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchAccessTypes()
                  .FetchStateProperties()
                  .FetchStatelessAccessControlList()
                  .FetchStatefulAcessControlLists();
    }

    public static IQueryable<SecurableClassDefinition> FetchAccessTypes (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchMany (SecurableClassDefinition.SelectAccessTypeReferences()).ThenFetchOne (r => r.AccessType);
    }

    public static IQueryable<SecurableClassDefinition> FetchStateProperties (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchMany (SecurableClassDefinition.SelectStatePropertyReferences())
                  .ThenFetchOne (r => r.StateProperty)
                  .ThenFetchMany (StatePropertyDefinition.SelectDefinedStates());
    }

    public static IQueryable<SecurableClassDefinition> FetchStatelessAccessControlList (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchOne (cd => cd.StatelessAccessControlList)
                  .ThenFetchMany (acl => acl.AccessControlEntries)
                  .ThenFetchMany (AccessControlEntry.SelectPermissions());
    }

    public static IQueryable<SecurableClassDefinition> FetchStatefulAcessControlLists (this IQueryable<SecurableClassDefinition> query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      return query.FetchMany (cd => cd.StatefulAccessControlLists)
                  .ThenFetchMany (acl => acl.AccessControlEntries)
                  .ThenFetchMany (AccessControlEntry.SelectPermissions())
                  .FetchMany (cd => cd.StatefulAccessControlLists)
                  .ThenFetchMany (StatefulAccessControlList.SelectStateCombinations())
                  .ThenFetchMany (StateCombination.SelectStateUsages());
    }
  }
}