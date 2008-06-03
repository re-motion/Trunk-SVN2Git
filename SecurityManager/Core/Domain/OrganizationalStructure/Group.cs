/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Group")]
  [PermanentGuid ("AA1761A4-226C-4ebe-91F0-8FFF4974B175")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Group : OrganizationalStructureObject
  {
    // types

    public enum Methods
    {
      //Create
      Search
    }

    // TODO: Rewrite with test
    protected class GroupSecurityStrategy : ObjectSecurityStrategy
    {
      private Group _group;

      public GroupSecurityStrategy (Group group)
          : base (group)
      {
        ArgumentUtility.CheckNotNull ("group", group);

        _group = group;
      }

      public override bool HasAccess (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes)
      {
        //TODO: if (!_group.IsDiscarded && _group.State == StateType.New)
        // Move ObjectSecurityAdapter into RPA and add IsDiscarded check.
        if (_group.IsDiscarded || _group.State == StateType.New)
          return true;

        return base.HasAccess (securityProvider, user, requiredAccessTypes);
      }
    }

    // static members and constants

    internal static Group NewObject ()
    {
      return NewObject<Group>().With();
    }

    public new static Group GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Group> (id);
    }

    public static DomainObjectCollection FindByTenantID (ObjectID tenantID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);

      Query query = new Query ("Remotion.SecurityManager.Domain.OrganizationalStructure.Group.FindByTenantID");
      query.Parameters.Add ("@tenantID", tenantID);

      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
    }

    public static Group FindByUnqiueIdentifier (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      Query query = new Query ("Remotion.SecurityManager.Domain.OrganizationalStructure.Group.FindByUnqiueIdentifier");
      query.Parameters.Add ("@uniqueIdentifier", uniqueIdentifier);

      DomainObjectCollection groups = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      if (groups.Count == 0)
        return null;

      return (Group) groups[0];
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static Group Create ()
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateGroup ();
    //}

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    // member fields

    // construction and disposing

    protected Group ()
    {
      UniqueIdentifier = Guid.NewGuid().ToString();
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (MaximumLength = 10)]
    public abstract string ShortName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    [DBBidirectionalRelation ("Children")]
    [SearchAvailableObjectsServiceType (typeof (GroupPropertiesSearchService))]
    public abstract Group Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<Group> Children { get; }

    [DBBidirectionalRelation ("Groups")]
    public abstract GroupType GroupType { get; set; }

    [DemandPropertyWritePermission (SecurityManagerAccessTypes.AssignRole)]
    [DBBidirectionalRelation ("Group")]
    public abstract ObjectList<Role> Roles { get; }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("SpecificGroup")]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    public override string DisplayName
    {
      get
      {
        if (StringUtility.IsNullOrEmpty (ShortName))
          return Name;
        else
          return string.Format ("{0} ({1})", ShortName, Name);
      }
    }

    // TODO: UnitTests
    public List<Group> GetPossibleParentGroups (ObjectID tenantID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);

      List<Group> groups = new List<Group>();

      foreach (Group group in FindByTenantID (tenantID))
      {
        if ((!Children.Contains (group.ID)) && (group.ID != ID))
          groups.Add (group);
      }
      return groups;
    }

    protected override string GetOwningTenant ()
    {
      return Tenant == null ? null : Tenant.UniqueIdentifier;
    }

    protected override string GetOwningGroup ()
    {
      return UniqueIdentifier;
    }
  }
}
