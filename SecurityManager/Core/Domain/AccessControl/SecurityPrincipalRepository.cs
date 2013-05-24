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
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Cache-based implementation of the <see cref="ISecurityPrincipalRepository"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class SecurityPrincipalRepository : RepositoryBase<SecurityPrincipalRepository.Data>, ISecurityPrincipalRepository
  {
    public class Data : RevisionBasedData
    {
      public readonly ICache<string, User> Users;

      internal Data (int revision)
        : base (revision)
      {
        Users = CacheFactory.CreateWithLazyLocking<string, User>();
      }
    }

    public SecurityPrincipalRepository (IRevisionProvider revisionProvider)
      : base (revisionProvider)
    {
    }

    public User GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      var cachedData = GetCachedData();
      return cachedData.Users.GetOrCreateValue (userName, GetUserInternal);
    }

    protected override Data LoadData (int revision)
    {
      return new Data (revision);
    }

    private User GetUserInternal (string userName)
    {
      var clientTransaction = ClientTransaction.CreateRootTransaction();
      using (clientTransaction.EnterNonDiscardingScope())
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
      QueryFactory.CreateLinqQuery<Position>().AsEnumerable().FirstOrDefault();
// ReSharper restore ReturnValueOfPureMethodIsNotUsed
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}