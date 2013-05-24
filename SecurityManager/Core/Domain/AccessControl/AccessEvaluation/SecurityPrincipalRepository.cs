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

    private static readonly ILog s_log = LogManager.GetLogger (MethodInfo.GetCurrentMethod().DeclaringType);
    private static readonly QueryCache s_queryCache = new QueryCache();
    
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
      s_log.Info ("Reset SecurityContextRepository cache.");
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
        LoadPosititions (clientTransaction);
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
        return s_queryCache.ExecuteCollectionQuery<User> (
            clientTransaction,
            MethodInfo.GetCurrentMethod().Name,
            users => users.Where (u => u.UserName == userName).Select (u => u)
                          .FetchOne (u => u.Tenant)
                          .FetchMany (u => u.Roles).ThenFetchOne (r => r.Group)
                          .FetchMany (User.SelectSubstitutions()).ThenFetchOne (s => s.SubstitutedRole).ThenFetchOne (r => r.Group))
                           .AsEnumerable()
                           .Single (() => CreateAccessControlException ("The user '{0}' could not be found.", userName));
      }
    }

    private void LoadPosititions (ClientTransaction clientTransaction)
    {
      using (StopwatchScope.CreateScope (
          s_log,
          LogLevel.Debug,
          "Fetched positions into SecurityPrincipalRepository. Time taken: {elapsed:ms}ms"))
      {
        s_queryCache.ExecuteCollectionQuery<Position> (
            clientTransaction,
            MethodInfo.GetCurrentMethod().Name,
            positions => positions);
      }
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}