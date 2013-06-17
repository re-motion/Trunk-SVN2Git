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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
using Remotion.Logging;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// Cache-based implementation of the <see cref="ISecurityPrincipalRepository"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class SecurityPrincipalRepository
      : RepositoryBase<SecurityPrincipalRepository.Data, RevisionKey, Int32RevisionValue>,
        ISecurityPrincipalRepository
  {
    public class Data : RevisionBasedData
    {
      public readonly ICache<string, User> Users;

      internal Data (Int32RevisionValue revision)
          : base (revision)
      {
        Users = CacheFactory.CreateWithLazyLocking<string, User>();
      }
    }

    private const string c_userNameParameter = "<userName>";

    private static readonly ILog s_log = LogManager.GetLogger (MethodInfo.GetCurrentMethod().DeclaringType);

    // Note: Parsing the query takes about 1/6 of the total query time when connected to a local database instance.
    // Unfortunately, the first query also causes the initialization of various caches in re-store, 
    // an operation that cannot be easily excluded from the meassured parsing time. Therefor, the cache mainly helps to alleviate any concerns 
    // about the cost associated with this part of the cache initialization.
    private static readonly QueryCache s_queryCache = new QueryCache();

    public SecurityPrincipalRepository (IDomainRevisionProvider revisionProvider)
        : base (revisionProvider)
    {
    }

    public User GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      var cachedData = GetCachedData (new RevisionKey());
      return cachedData.Users.GetOrCreateValue (userName, GetUserInternal);
    }

    protected override Data LoadData (Int32RevisionValue revision)
    {
      s_log.Info ("Reset SecurityPrincipalRepository cache.");
      return new Data (revision);
    }

    private User GetUserInternal (string userName)
    {
      using (StopwatchScope.CreateScope (
          s_log,
          LogLevel.Info,
          "Refreshed data in SecurityPrincipalRepository for user '" + userName + "'. Time taken: {elapsed:ms}ms"))
      {
        var clientTransaction = ClientTransaction.CreateRootTransaction();
        return LoadUser (clientTransaction, userName);
      }
    }

    private User LoadUser (ClientTransaction clientTransaction, string userName)
    {
      using (StopwatchScope.CreateScope (
          s_log,
          LogLevel.Debug,
          "Fetched user '" + userName + "' into SecurityPrincipalRepository. Time taken: {elapsed:ms}ms"))
      {
        var queryTemplate = s_queryCache.GetQuery<User> (
            MethodInfo.GetCurrentMethod().Name,
            users => users.Where (u => u.UserName == c_userNameParameter).Select (u => u)
                          .FetchMany (u => u.Roles)
                          .FetchMany (User.SelectSubstitutions()).ThenFetchOne (s => s.SubstitutedRole));

        var query = queryTemplate.CreateCopyFromTemplate (new Dictionary<object, object> { { c_userNameParameter, userName } });
        return clientTransaction.QueryManager.GetCollection<User> (query)
                                .AsEnumerable()
                                .Single (() => CreateAccessControlException ("The user '{0}' could not be found.", userName));
      }
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}