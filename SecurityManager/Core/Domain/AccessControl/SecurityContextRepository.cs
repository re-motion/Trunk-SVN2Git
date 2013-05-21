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
using System.Threading;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Cache-based implementation of the <see cref="ISecurityContextRepository"/> interface.
  /// </summary>
  public class SecurityContextRepository : ISecurityContextRepository
  {
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private DateTime _nextRevisionCheckInUtc;
    private readonly TimeSpan _revisionCheckInterval = TimeSpan.FromSeconds (1);
    private int _revision;

    private Dictionary<string, IDomainObjectHandle<Tenant>> _tenantCache;
    private Dictionary<string, IDomainObjectHandle<Group>> _groupCache;
    private Dictionary<string, IDomainObjectHandle<User>> _userCache;
    private Dictionary<EnumWrapper, IDomainObjectHandle<AbstractRoleDefinition>> _abstractRoleCache;
    private Dictionary<string, SecurableClassDefinition> _classCache;

    public SecurityContextRepository ()
    {
      InitializeCache (GetRevision());
    }

    public IDomainObjectHandle<Tenant> GetTenant (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      RefreshOnDemand();
      _lock.EnterReadLock();
      try
      {
        var tenant = _tenantCache.GetValueOrDefault (uniqueIdentifier);
        if (tenant == null)
          throw CreateAccessControlException ("The tenant '{0}' could not be found.", uniqueIdentifier);
        return tenant;
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    public IDomainObjectHandle<Group> GetGroup (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      RefreshOnDemand();
      _lock.EnterReadLock();
      try
      {
        var group = _groupCache.GetValueOrDefault (uniqueIdentifier);
        if (group == null)
          throw CreateAccessControlException ("The group '{0}' could not be found.", uniqueIdentifier);
        return group;
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    public IDomainObjectHandle<User> GetUser (string userName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("userName", userName);

      RefreshOnDemand();
      _lock.EnterReadLock();
      try
      {
        var user = _userCache.GetValueOrDefault (userName);
        if (user == null)
          throw CreateAccessControlException ("The user '{0}' could not be found.", userName);
        return user;
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    public IDomainObjectHandle<AbstractRoleDefinition> GetAbstractRole (EnumWrapper abstractRoleName)
    {
      RefreshOnDemand();
      _lock.EnterReadLock();
      try
      {
        var abstractRole = _abstractRoleCache.GetValueOrDefault (abstractRoleName);
        if (abstractRole == null)
          throw CreateAccessControlException ("The abstract role '{0}' could not be found.", abstractRoleName);
        return abstractRole;
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    public SecurableClassDefinition GetClass (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      RefreshOnDemand();
      _lock.EnterReadLock();
      try
      {
        var @class = _classCache.GetValueOrDefault (name);
        if (@class == null)
          throw CreateAccessControlException ("The securable class '{0}' could not be found.", name);
        return @class;
      }
      finally
      {
        _lock.ExitReadLock();
      }
    }

    private void RefreshOnDemand ()
    {
      if (DateTime.UtcNow >= _nextRevisionCheckInUtc)
      {
        _nextRevisionCheckInUtc = DateTime.UtcNow.Add (_revisionCheckInterval);
        Refresh();
      }
    }

    private void Refresh ()
    {
      _lock.EnterUpgradeableReadLock();
      try
      {
        var revision = GetRevision();
        if (revision != _revision)
          InitializeCache (revision);
      }
      finally
      {
        _lock.ExitUpgradeableReadLock();
      }
    }

    private void InitializeCache (int revision)
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        var newTenantCache = QueryFactory.CreateLinqQuery<Tenant>()
                                   .Select (t => new { Key = t.UniqueIdentifier, Value = t.ID })
                                   .ToDictionary (t => t.Key, t => t.Value.GetHandle<Tenant>());

        var newGroupCache = QueryFactory.CreateLinqQuery<Group>()
                                   .Select (g => new { Key = g.UniqueIdentifier, Value = g.ID })
                                   .ToDictionary (g => g.Key, g => g.Value.GetHandle<Group>());

        var newUserCache = QueryFactory.CreateLinqQuery<User>()
                                   .Select (u => new { Key = u.UserName, Value = u.ID })
                                   .ToDictionary (u => u.Key, u => u.Value.GetHandle<User>());

        var newAbstractRoleCache = QueryFactory.CreateLinqQuery<AbstractRoleDefinition>()
                                   .Select (r => new { Key = r.Name, Value = r.ID })
                                   .ToDictionary (r => EnumWrapper.Get (r.Key), r => r.Value.GetHandle<AbstractRoleDefinition>());

        var newClassCache = QueryFactory.CreateLinqQuery<SecurableClassDefinition>().Select (c => c)
                                        .FetchStateProperties()
                                        .FetchOne (cd => cd.StatelessAccessControlList)
                                        .FetchMany (cd => cd.StatefulAccessControlLists)
                                        .ThenFetchMany (StatefulAccessControlList.SelectStateCombinations())
                                        .ThenFetchMany (StateCombination.SelectStateUsages()).
                                         ToDictionary (c => c.Name);

        try
        {
          _lock.EnterWriteLock();

          _revision = revision;
          _tenantCache = newTenantCache;
          _groupCache = newGroupCache;
          _userCache = newUserCache;
          _abstractRoleCache = newAbstractRoleCache;
          _classCache = newClassCache;
        }
        finally
        {
          _lock.ExitWriteLock();
        }
      }
    }

    private int GetRevision ()
    {
      return (int) ClientTransaction.CreateRootTransaction().QueryManager.GetScalar (Revision.GetGetRevisionQuery());
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}