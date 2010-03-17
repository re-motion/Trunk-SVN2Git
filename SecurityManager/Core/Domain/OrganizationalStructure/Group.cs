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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Linq;

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

      public override bool HasAccess (ISecurityProvider securityProvider, ISecurityPrincipal principal, params AccessType[] requiredAccessTypes)
      {
        //TODO: if (!_group.IsDiscarded && _group.State == StateType.New)
        // Move ObjectSecurityAdapter into RPA and add IsDiscarded check.
        if (_group.IsDiscarded || _group.State == StateType.New)
          return true;

        return base.HasAccess (securityProvider, principal, requiredAccessTypes);
      }
    }

    // static members and constants

    internal static Group NewObject ()
    {
      return NewObject<Group>();
    }

    public new static Group GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Group> (id);
    }

    public static ObjectList<Group> FindByTenantID (ObjectID tenantID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);

      var result = from g in QueryFactory.CreateLinqQuery<Group>()
                   where g.Tenant.ID == tenantID
                   orderby g.Name, g.ShortName
                   select g;
      
      return result.ToObjectList();
    }

    public static Group FindByUnqiueIdentifier (string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      var result = from g in QueryFactory.CreateLinqQuery<Group>()
                   where g.UniqueIdentifier == uniqueIdentifier
                   select g;

      return result.ToArray().SingleOrDefault();
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

    private DomainObjectDeleteHandler _deleteHandler;

    // construction and disposing

    protected Group ()
    {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
      UniqueIdentifier = Guid.NewGuid().ToString();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    // methods and properties

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (MaximumLength = 20)]
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

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (AccessControlEntries, Roles);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete ();
    }

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

    /// <summary>
    /// Gets the <see cref="Group"/> and all of its <see cref="Children"/>, provided the user as read access for the respective object.
    /// </summary>
    /// <remarks>This collection will be empty, if the user does not have <see cref="GeneralAccessTypes.Read"/> access on the current object.</remarks>
    public IEnumerable<Group> GetHierachy ()
    {
      var securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      if (!securityClient.HasAccess (this, AccessType.Get (GeneralAccessTypes.Read)))
        return new Group[0];

      return new[] { this }.Concat (Children.SelectMany (c => c.GetHierachy()));
    }
  }
}
