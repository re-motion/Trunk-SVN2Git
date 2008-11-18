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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.AccessControl.AccessControlEntry")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessControlEntry : AccessControlObject
  {
    // types

    // static members and constants

    public static AccessControlEntry NewObject ()
    {
      return NewObject<AccessControlEntry>().With();
    }

    public new static AccessControlEntry GetObject (ObjectID id)
    {
      return GetObject<AccessControlEntry> (id);
    }

    // member fields

    private ObjectList<Permission> _permissionsToBeDeleted;
    private SecurityTokenMatcher _matcher;

    // construction and disposing

    protected AccessControlEntry ()
    {
      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      TenantCondition = TenantCondition.None;
      GroupCondition = GroupCondition.None;
      UserCondition = UserCondition.None;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor

      _matcher = new SecurityTokenMatcher (this);
    }

    // methods and properties

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);

      if (loadMode == LoadMode.WholeDomainObjectInitialized)
        _matcher = new SecurityTokenMatcher (this);
    }

    public void Touch ()
    {
      if (State == StateType.Unchanged)
        MarkAsChanged();
    }

    public abstract int Index { get; set; }

    [DisableEnumValues (typeof (AccessControlEntryPropertiesEnumerationValueFilter))]
    public abstract TenantCondition TenantCondition { get; set; }

    public abstract TenantHierarchyCondition TenantHierarchyCondition { get; set; }

    [DisableEnumValues (typeof (AccessControlEntryPropertiesEnumerationValueFilter))]
    public abstract GroupCondition GroupCondition { get; set; }

    public abstract GroupHierarchyCondition GroupHierarchyCondition { get; set; }

    [DisableEnumValues(typeof (AccessControlEntryPropertiesEnumerationValueFilter))]
    public abstract UserCondition UserCondition { get; set; }

    [SearchAvailableObjectsServiceType (typeof (AccessControlEntryPropertiesSearchService))]
    public abstract Tenant SpecificTenant { get; set; }

    [SearchAvailableObjectsServiceType (typeof (AccessControlEntryPropertiesSearchService))]
    [DBBidirectionalRelation ("AccessControlEntries")]
    public abstract Group SpecificGroup { get; set; }

    [SearchAvailableObjectsServiceType (typeof (AccessControlEntryPropertiesSearchService))]
    [DBBidirectionalRelation ("AccessControlEntries")]
    public abstract GroupType SpecificGroupType { get; set; }

    [SearchAvailableObjectsServiceType (typeof (AccessControlEntryPropertiesSearchService))]
    [DBBidirectionalRelation ("AccessControlEntries")]
    public abstract Position SpecificPosition { get; set; }

    [SearchAvailableObjectsServiceType (typeof (AccessControlEntryPropertiesSearchService))]
    [DBBidirectionalRelation ("AccessControlEntries")]
    public abstract User SpecificUser { get; set; }

    [SearchAvailableObjectsServiceType (typeof (AccessControlEntryPropertiesSearchService))]
    [DBBidirectionalRelation ("AccessControlEntries")]
    public abstract AbstractRoleDefinition SpecificAbstractRole { get; set; }

    [DBBidirectionalRelation ("AccessControlEntries")]
    public abstract AccessControlList AccessControlList { get; set; }

    [DBBidirectionalRelation ("AccessControlEntry", SortExpression = "[Index] ASC")]
    [ObjectBinding (ReadOnly = true)]
    public virtual ObjectList<Permission> Permissions
    {
      get { return new ObjectList<Permission> (CurrentProperty.GetValue<ObjectList<Permission>>(), true); }
    }

    private ObjectList<Permission> GetPermissions ()
    {
      return Properties[typeof (AccessControlEntry), "Permissions"].GetValue<ObjectList<Permission>>();
    }

    public AccessTypeDefinition[] GetAllowedAccessTypes ()
    {
      return Permissions.Where (p => (p.Allowed.HasValue && p.Allowed.Value)).Select (p => p.AccessType).ToArray();
    }

    public AccessTypeDefinition[] GetDeniedAccessTypes ()
    {
      return Permissions.Where (p => (p.Allowed.HasValue && !p.Allowed.Value)).Select (p => p.AccessType).ToArray();
    }

    public void AttachAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      if (FindPermission (accessType) != null)
      {
        throw new ArgumentException (
            string.Format ("The access type '{0}' has already been attached to this access control entry.", accessType.Name), "accessType");
      }

      var permission = Permission.NewObject();
      permission.AccessType = accessType;
      permission.Allowed = null;
      var permissions = GetPermissions();
      permissions.Add (permission);
      if (permissions.Count == 1)
        permission.Index = 0;
      else
        permission.Index = permissions[permissions.Count - 2].Index + 1;
      Touch();
    }

    public void AllowAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      var permission = GetPermission (accessType);
      permission.Allowed = true;
    }

    public void DenyAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      var permission = GetPermission (accessType);
      permission.Allowed = false;
    }

    public void RemoveAccess (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      var permission = GetPermission (accessType);
      permission.Allowed = null;
    }

    public bool MatchesToken (SecurityToken token)
    {
      ArgumentUtility.CheckNotNull ("token", token);

      return _matcher.MatchesToken (token);
    }

    private Permission GetPermission (AccessTypeDefinition accessType)
    {
      var permission = FindPermission (accessType);
      if (permission == null)
      {
        throw new ArgumentException (
            string.Format ("The access type '{0}' is not assigned to this access control entry.", accessType.Name), "accessType");
      }

      return permission;
    }

    private Permission FindPermission (AccessTypeDefinition accessType)
    {
      return Permissions.Where (p => p.AccessType.ID == accessType.ID).SingleOrDefault();
    }

    //TODO: Rewrite with test

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _permissionsToBeDeleted = Permissions.Clone();
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (var permission in _permissionsToBeDeleted)
        permission.Delete();

      _permissionsToBeDeleted = null;
    }

    public AccessControlEntryValidationResult Validate ()
    {
      var result = new AccessControlEntryValidationResult();

      if (State != StateType.Deleted)
      {
        if (TenantCondition == TenantCondition.SpecificTenant && SpecificTenant == null)
          result.SetSpecificTenantMissing();
      }

      return result;
    }

    protected override void OnCommitting (EventArgs args)
    {
      AccessControlEntryValidationResult result = Validate();
      if (!result.IsValid)
      {
        if (result.IsSpecificTenantMissing)
        {
          throw new ConstraintViolationException (
              "The access control entry has the Tenant property set to SpecificTenant, but no Tenant is assigned.");
        }

        //TODO: Move the message into the validation logic.
        throw new ConstraintViolationException ("The access control entry is in an invalid state.");
      }

      if (State != StateType.Deleted)
      {
        if (TenantCondition != TenantCondition.SpecificTenant)
          SpecificTenant = null;

        if (GroupCondition != GroupCondition.SpecificGroup)
          SpecificGroup = null;

        if (GroupCondition != GroupCondition.AnyGroupWithSpecificGroupType && GroupCondition != GroupCondition.BranchOfOwningGroup)
          SpecificGroupType = null;

        if (UserCondition != UserCondition.SpecificUser)
          SpecificUser = null;

        if (UserCondition != UserCondition.SpecificPosition)
          SpecificPosition = null;
      }

      base.OnCommitting (args);
    }
  }
}