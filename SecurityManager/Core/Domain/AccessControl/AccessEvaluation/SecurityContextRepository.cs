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
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Linq.EagerFetching;
using Remotion.Logging;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation
{
  /// <summary>
  /// Cache-based implementation of the <see cref="ISecurityContextRepository"/> interface.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class SecurityContextRepository : RepositoryBase<SecurityContextRepository.Data>, ISecurityContextRepository
  {
    public class Data : RevisionBasedData
    {
      public readonly Dictionary<string, IDomainObjectHandle<Tenant>> Tenants;
      public readonly Dictionary<string, IDomainObjectHandle<Group>> Groups;
      public readonly Dictionary<string, IDomainObjectHandle<User>> Users;
      public readonly Dictionary<string, IDomainObjectHandle<Position>> Positions;
      public readonly Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> AbstractRoles;
      public readonly Dictionary<string, SecurableClassDefinitionData> Classes;
      public readonly Dictionary<IDomainObjectHandle<StatePropertyDefinition>, ReadOnlyCollectionDecorator<string>> StatePropertyValues;

      internal Data (
          int revision,
          Dictionary<string, IDomainObjectHandle<Tenant>> tenants,
          Dictionary<string, IDomainObjectHandle<Group>> groups,
          Dictionary<string, IDomainObjectHandle<User>> users,
          Dictionary<string, IDomainObjectHandle<Position>> positions,
          Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> abstractRoles,
          Dictionary<string, SecurableClassDefinitionData> classes,
          Dictionary<IDomainObjectHandle<StatePropertyDefinition>, ReadOnlyCollectionDecorator<string>> statePropertyValues)
          : base (revision)
      {
        Tenants = tenants;
        Groups = groups;
        Users = users;
        Positions = positions;
        AbstractRoles = abstractRoles;
        Classes = classes;
        StatePropertyValues = statePropertyValues;
      }
    }

    private static readonly ILog s_log = LogManager.GetLogger (MethodInfo.GetCurrentMethod().DeclaringType);
    private static readonly ICache<string, IQuery> s_queryCache = CacheFactory.CreateWithLocking<string, IQuery>();

    public SecurityContextRepository (IRevisionProvider revisionProvider)
        : base (revisionProvider)
    {
    }

    public IDomainObjectHandle<Tenant> GetTenant (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var cachedData = GetCachedData();
      var tenant = cachedData.Tenants.GetValueOrDefault (uniqueIdentifier);
      if (tenant == null)
        throw CreateAccessControlException ("The tenant '{0}' could not be found.", uniqueIdentifier);
      return tenant;
    }

    public IDomainObjectHandle<Group> GetGroup (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var cachedData = GetCachedData();
      var group = cachedData.Groups.GetValueOrDefault (uniqueIdentifier);
      if (group == null)
        throw CreateAccessControlException ("The group '{0}' could not be found.", uniqueIdentifier);
      return group;
    }

    public IDomainObjectHandle<User> GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      var cachedData = GetCachedData();
      var user = cachedData.Users.GetValueOrDefault (userName);
      if (user == null)
        throw CreateAccessControlException ("The user '{0}' could not be found.", userName);
      return user;
        }

    public IDomainObjectHandle<Position> GetPosition (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var cachedData = GetCachedData();
      var position = cachedData.Positions.GetValueOrDefault (uniqueIdentifier);
      if (position == null)
        throw CreateAccessControlException ("The position '{0}' could not be found.", uniqueIdentifier);
      return position;
    }


    public IDomainObjectHandle<AbstractRoleDefinition> GetAbstractRole (EnumWrapper name)
    {
      ArgumentUtility.CheckNotNull ("name", name);

      var cachedData = GetCachedData();
      var abstractRole = cachedData.AbstractRoles.GetValueOrDefault (name);
      if (abstractRole == null)
        throw CreateAccessControlException ("The abstract role '{0}' could not be found.", name);
      return abstractRole;
    }

    public SecurableClassDefinitionData GetClass (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var cachedData = GetCachedData();
      var @class = cachedData.Classes.GetValueOrDefault (name);
      if (@class == null)
        throw CreateAccessControlException ("The securable class '{0}' could not be found.", name);
      return @class;
    }

    public ReadOnlyCollectionDecorator<string> GetStatePropertyValues (IDomainObjectHandle<StatePropertyDefinition> stateProperty)
    {
      ArgumentUtility.CheckNotNull ("stateProperty", stateProperty);

      var cachedData = GetCachedData();
      var values = cachedData.StatePropertyValues.GetValueOrDefault (stateProperty);
      if (values == null)
        throw CreateAccessControlException ("The state property with ID '{0}' could not be found.", stateProperty);
      return values;
    }

    protected override Data LoadData (int revision)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        s_log.Info ("Reset SecurityContextRepository cache.");
        using (StopwatchScope.CreateScope (s_log, LogLevel.Info, "Refreshed data in SecurityContextRepository. Time taken: {elapsed:ms}ms"))
        {
          var tenants = LoadTenants();
          var groups = LoadGroups();
          var users = LoadUsers();
          var positions = LoadPositions();
          var abstractRoles = LoadAbstractRoles();
          var classes = BuildClassCache (
              LoadSecurableClassDefinitions(),
              LoadStatelessAccessControlLists(),
              LoadStatefulAccessControlLists());
          var statePropertyValues = LoadStatePropertyValues();

          return new Data (revision, tenants, groups, users, positions, abstractRoles, classes, statePropertyValues);
        }
      }
    }

    private Dictionary<string, IDomainObjectHandle<Tenant>> LoadTenants ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from t in QueryFactory.CreateLinqQuery<Tenant>()
                select new { Key = t.UniqueIdentifier, Value = t.ID.GetHandle<Tenant>() });

      using (CreateStopwatchScopeForQueryExecution ("tenants"))
      {
        return result.ToDictionary (t => t.Key, t => t.Value);
      }
    }

    private Dictionary<string, IDomainObjectHandle<Group>> LoadGroups ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from g in QueryFactory.CreateLinqQuery<Group>()
                select new { Key = g.UniqueIdentifier, Value = g.ID.GetHandle<Group>() });

      using (CreateStopwatchScopeForQueryExecution ("groups"))
      {
        return result.ToDictionary (g => g.Key, g => g.Value);
      }
    }

    private Dictionary<string, IDomainObjectHandle<User>> LoadUsers ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from u in QueryFactory.CreateLinqQuery<User>()
                select new { Key = u.UserName, Value = u.ID.GetHandle<User>() });

      using (CreateStopwatchScopeForQueryExecution ("users"))
      {
        return result.ToDictionary (u => u.Key, u => u.Value);
      }
    }

    private Dictionary<string, IDomainObjectHandle<Position>> LoadPositions ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from g in QueryFactory.CreateLinqQuery<Position>()
                select new { Key = g.UniqueIdentifier, Value = g.ID.GetHandle<Position>() });

      using (CreateStopwatchScopeForQueryExecution ("positions"))
      {
        return result.ToDictionary (g => g.Key, g => g.Value);
      }
    }

    private Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> LoadAbstractRoles ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from r in QueryFactory.CreateLinqQuery<AbstractRoleDefinition>()
                select new { Key = r.Name, Value = r.ID.GetHandle<AbstractRoleDefinition>() });

      using (CreateStopwatchScopeForQueryExecution ("abstract roles"))
      {
        return result.ToDictionary (r => EnumWrapper.Get (r.Key), r => r.Value);
      }
    }

    private Dictionary<ObjectID, Tuple<string, ObjectID>> LoadSecurableClassDefinitions ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from @class in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                select new
                       {
                           @class.ID,
                           @class.Name,
                           HasBaseClass = @class.BaseClass != null,
                           BaseClassID = @class.BaseClass.ID.Value,
                           BaseClassClassID = @class.BaseClass.ID.ClassID,
                       });

      using (CreateStopwatchScopeForQueryExecution ("securable classes"))
      {
        return result.ToDictionary (c => c.ID, c => Tuple.Create (c.Name, c.HasBaseClass ? new ObjectID (c.BaseClassClassID, c.BaseClassID) : null));
      }
    }

    private ILookup<ObjectID, StatefulAccessControlListData> LoadStatefulAccessControlLists ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from acl in QueryFactory.CreateLinqQuery<StatefulAccessControlList>()
                from sc in acl.GetStateCombinationsForQuery()
                from usage in sc.GetStateUsagesForQuery().DefaultIfEmpty()
                from propertyReference in acl.GetClassForQuery().GetStatePropertyReferencesForQuery().DefaultIfEmpty()
                select new
                       {
                           Class = acl.GetClassForQuery().ID,
                           StateCombination = sc.ID.GetHandle<StateCombination>(),
                           Acl = acl.ID.GetHandle<StatefulAccessControlList>(),
                           HasState = propertyReference != null,
                           StatePropertyID = propertyReference.StateProperty.ID.Value,
                           StatePropertyClassID = propertyReference.StateProperty.ID.ClassID,
                           StatePropertyName = propertyReference.StateProperty.Name,
                           StateValue = usage.StateDefinition.Name
                       });

      using (CreateStopwatchScopeForQueryExecution ("stateful ACLs"))
      {
        return result.GroupBy (
            row => new { row.Class, row.Acl, row.StateCombination },
            row => row.HasState
                       ? new State (
                             new ObjectID (row.StatePropertyClassID, row.StatePropertyID).GetHandle<StatePropertyDefinition>(),
                             row.StatePropertyName,
                             row.StateValue)
                       : null)
                     .ToLookup (g => g.Key.Class, g => new StatefulAccessControlListData (g.Key.Acl, g.Where (s => s != null)));
      }
    }

    private Dictionary<ObjectID, IDomainObjectHandle<StatelessAccessControlList>> LoadStatelessAccessControlLists ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from acl in QueryFactory.CreateLinqQuery<StatelessAccessControlList>()
                select new { Class = acl.GetClassForQuery().ID, Acl = acl.ID.GetHandle<StatelessAccessControlList>() });

      using (CreateStopwatchScopeForQueryExecution ("stateless ACLs"))
      {
        return result.ToDictionary (o => o.Class, o => o.Acl);
      }
    }

    private Dictionary<string, SecurableClassDefinitionData> BuildClassCache (
        IDictionary<ObjectID, Tuple<string, ObjectID>> classes,
        IDictionary<ObjectID, IDomainObjectHandle<StatelessAccessControlList>> statelessAcls,
        ILookup<ObjectID, StatefulAccessControlListData> statefulAcls)
    {
      return classes.ToDictionary (
          c => c.Value.Item1,
          c => new SecurableClassDefinitionData (
                   c.Value.Item2 != null ? classes[c.Value.Item2].Item1 : null,
                   statelessAcls.GetValueOrDefault (c.Key),
                   statefulAcls[c.Key]));
    }

    private Dictionary<IDomainObjectHandle<StatePropertyDefinition>, ReadOnlyCollectionDecorator<string>> LoadStatePropertyValues ()
    {
      var result = GetOrCreateQuery (
          MethodInfo.GetCurrentMethod(),
          () => from s in QueryFactory.CreateLinqQuery<StateDefinition>()
                select
                    new
                    {
                        PropertyHandle = s.StateProperty.ID.GetHandle<StatePropertyDefinition>(),
                        PropertyValue = s.Name
                    });

      using (CreateStopwatchScopeForQueryExecution ("state properties"))
      {
        var lookUp = result.ToLookup (o => o.PropertyHandle, o => o.PropertyValue);
        return lookUp.ToDictionary (o => o.Key, o => o.ToArray().AsReadOnly());
      }
    }

    private static StopwatchScope CreateStopwatchScopeForQueryParsing (string queryName)
    {
      return StopwatchScope.CreateScope (
          s_log,
          LogLevel.Debug,
          "Parsed query for SecurityContextRepository." + queryName + "(). Time taken: {elapsed:ms}ms");
    }

    private static StopwatchScope CreateStopwatchScopeForQueryExecution (string queryName)
    {
      return StopwatchScope.CreateScope (
          s_log,
          LogLevel.Debug,
          "Fetched " + queryName + " into SecurityContextRepository. Time taken: {elapsed:ms}ms");
    }

    private IEnumerable<T> GetOrCreateQuery<T> (MethodBase caller, Func<IQueryable<T>> queryCreator)
    {
      var executableQuery =
          (IExecutableQuery<IEnumerable<T>>) s_queryCache.GetOrCreateValue (caller.Name, key => CreateExecutableQuery (key, queryCreator));

      return executableQuery.Execute (ClientTransaction.Current.QueryManager);
    }

    private static IExecutableQuery<IEnumerable<T>> CreateExecutableQuery<T> (string key, Func<IQueryable<T>> queryCreator)
    {
      // Note: Parsing the query takes about 1/3 of the total query time when connected to a local database instance.
      // Unfortunately, the first query also causes the initialization of various caches in re-store, 
      // an operation that cannot be easily excluded from the meassured parsing time. Therefor, the cache mainly helps to alleviate any concerns 
      // about the cost associated with this part of the cache initialization.
      using (CreateStopwatchScopeForQueryParsing (key))
      {
        var queryable = (DomainObjectQueryable<T>) queryCreator();
        var queryExecutor = queryable.GetExecutor();
        var queryModel = queryable.Provider.GenerateQueryModel (queryable.Expression);
        return queryExecutor.QueryGenerator.CreateSequenceQuery<T> (
            "<dynamic query>",
            queryExecutor.StorageProviderDefinition,
            queryModel,
            Enumerable.Empty<FetchQueryModelBuilder>());
      }
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}