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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessResolver : IAccessResolver
  {
    public AccessType[] GetAccessTypes (IDomainObjectHandle<AccessControlList> aclHandle, SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("aclHandle", aclHandle);
      ArgumentUtility.CheckNotNull ("token", token);
      
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      using (clientTransaction.EnterDiscardingScope())
      {
        QueryFactory.CreateLinqQuery<AccessTypeDefinition>().ToArray();
        var acl = QueryFactory.CreateLinqQuery<AccessControlList>().Where (o => o.ID == aclHandle.ObjectID).Select (o => o)
                              .FetchMany (o => o.AccessControlEntries)
                              .ThenFetchMany (AccessControlEntry.SelectPermissions())
                              .ToList().Single();

        AccessInformation accessInformation = acl.GetAccessTypes (token);
        return Array.ConvertAll (accessInformation.AllowedAccessTypes, ConvertToAccessType);
      }
    }

    private AccessType ConvertToAccessType (AccessTypeDefinition accessTypeDefinition)
    {
      return AccessType.Get (EnumWrapper.Get (accessTypeDefinition.Name));
    }
  }
}