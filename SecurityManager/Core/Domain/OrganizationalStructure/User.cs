// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Remotion.Context;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.ObjectBinding;
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

    private DomainObjectDeleteHandler _deleteHandler;

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

    [DBBidirectionalRelation ("SubstitutingUser")]
    protected abstract ObjectList<Substitution> SubstitutingFor { get; }

    [DemandPropertyWritePermission (SecurityManagerAccessTypes.AssignSubstitute)]
    [DBBidirectionalRelation ("SubstitutedUser")]
    public abstract ObjectList<Substitution> SubstitutedBy { get; }

    public IList<Substitution> GetActiveSubstitutions ()
    {
      return SubstitutingFor.Where (s => s.IsActive).ToArray();
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (AccessControlEntries, Roles, SubstitutingFor, SubstitutedBy);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete();
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
