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
using Remotion.FunctionalProgramming;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class SecurityPrincipalRepository : ISecurityPrincipalRepository
  {
    public User GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        PrefetchPositions();
        return QueryFactory.CreateLinqQuery<User>().Where (u => u.UserName == userName).Select (u => u)
                           .FetchOne (u => u.Tenant)
                           .FetchMany (u => u.Roles).ThenFetchOne (r => r.Group)
                           .FetchMany (User.SelectSubstitutions()).ThenFetchOne (s => s.SubstitutedRole).ThenFetchOne (r => r.Group)
                           .ToList().Single (() => CreateAccessControlException ("The user '{0}' could not be found.", userName));
      }
    }

    private void PrefetchPositions ()
    {
// ReSharper disable ReturnValueOfPureMethodIsNotUsed
      QueryFactory.CreateLinqQuery<Position>().ToList();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}