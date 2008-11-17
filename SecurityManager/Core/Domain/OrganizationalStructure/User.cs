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
using System.Linq;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.User")]
  [PermanentGuid ("759DA370-E2C4-4221-B878-BE378C916042")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class User : OrganizationalStructureObject
  {
    public enum Methods
    {
      //Create
      Search
    }

    private static readonly string s_currentKey = typeof (User).AssemblyQualifiedName + "_Current";

    public static User Current
    {
      get
      {
        ObjectID userID = (ObjectID) SafeContext.Instance.GetData (s_currentKey);
        if (userID == null)
          return null;
        return GetObject (userID);
      }
      set
      {
        if (value == null)
          SafeContext.Instance.SetData (s_currentKey, null);
        else
          SafeContext.Instance.SetData (s_currentKey, value.ID);
      }
    }

    internal static User NewObject ()
    {
      return NewObject<User> ().With ();
    }

    public static new User GetObject (ObjectID id)
    {
      return DomainObject.GetObject<User> (id);
    }

    public static User FindByUserName (string userName)
    {
      ArgumentUtility.CheckNotNull ("userName", userName);

      var result = from u in QueryFactory.CreateLinqQuery<User>()
                   where u.UserName == userName
                   select u;

      return result.ToArray().SingleOrDefault();
    }

    public static ObjectList<User> FindByTenantID (ObjectID tenantID)
    {
      ArgumentUtility.CheckNotNull ("tenantID", tenantID);

      var result = from u in QueryFactory.CreateLinqQuery<User>()
                   where u.Tenant.ID == tenantID
                   orderby u.LastName, u.FirstName
                   select u;

      return result.ToObjectList ();
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static User Create ()
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateUser ();
    //}

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    private ObjectList<AccessControlEntry> _accessControlEntriesToBeDeleted;
    private ObjectList<Role> _rolesToBeDeleted;

    protected User ()
    {
    }

    [StringProperty (MaximumLength = 100)]
    public abstract string Title { get; set; }

    [StringProperty (MaximumLength = 100)]
    public abstract string FirstName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UserName { get; set; }

    [DemandPropertyWritePermission (SecurityManagerAccessTypes.AssignRole)]
    [DBBidirectionalRelation ("User")]
    public abstract ObjectList<Role> Roles { get; }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    [Mandatory]
    [SearchAvailableObjectsServiceType(typeof (UserPropertiesSearchService))]
    public abstract Group OwningGroup { get; set; }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("SpecificUser")]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _accessControlEntriesToBeDeleted = AccessControlEntries.Clone ();
      _rolesToBeDeleted = Roles.Clone ();
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      foreach (AccessControlEntry accessControlEntry in _accessControlEntriesToBeDeleted)
        accessControlEntry.Delete ();
      _accessControlEntriesToBeDeleted = null;

      foreach (Role role in _rolesToBeDeleted)
        role.Delete ();
      _rolesToBeDeleted = null;
    }

    public override string DisplayName
    {
      get { return GetFormattedName (); }
    }

    private string GetFormattedName ()
    {
      string formattedName = LastName;

      if (!StringUtility.IsNullOrEmpty (FirstName))
        formattedName += " " + FirstName;

      if (!StringUtility.IsNullOrEmpty (Title))
        formattedName += ", " + Title;

      return formattedName;
    }

    protected override string GetOwner ()
    {
      return UserName;
    }

    protected override string GetOwningTenant ()
    {
      return Tenant == null ? null : Tenant.UniqueIdentifier;
    }

    protected override string GetOwningGroup ()
    {
      return OwningGroup == null ? null : OwningGroup.UniqueIdentifier;
    }
  }
}
