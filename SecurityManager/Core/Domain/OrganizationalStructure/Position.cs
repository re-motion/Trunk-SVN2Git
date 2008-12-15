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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Position")]
  [PermanentGuid ("5BBE6C4D-DC88-4a27-8BFF-0AC62EE34333")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Position : OrganizationalStructureObject
  {
    private DomainObjectDeleteHandler _deleteHandler;

    public enum Methods
    {
      //Create
      Search
    }

    internal static Position NewObject ()
    {
      return NewObject<Position> ().With ();
    }

    public static ObjectList<Position> FindAll ()
    {
      var result = from p in QueryFactory.CreateLinqQuery<Position>()
                   orderby p.Name
                   select p;

      return result.ToObjectList ();
    }

    [DemandMethodPermission (SecurityManagerAccessTypes.AssignRole)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Dummy_AssignRole ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    //[DemandMethodPermission (GeneralAccessTypes.Create)]
    //public static Position Create ()
    //{
    //  return SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreatePosition ();
    //}

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void Search ()
    {
      throw new NotImplementedException ("This method is only intended for framework support and should never be called.");
    }

    protected Position ()
    {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
      UniqueIdentifier = Guid.NewGuid ().ToString ();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string UniqueIdentifier { get; set; }

    [PermanentGuid ("5C31F600-88F3-4ff7-988C-0E45A857AB4B")]
    public abstract Delegation Delegation { get; set; }

    [DBBidirectionalRelation ("Position")]
    public abstract ObjectList<GroupTypePosition> GroupTypes { get; }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("Position")]
    protected abstract ObjectList<Role> Roles { get; }

    // Must not be private because PermissionReflection would not work with derived classes.
    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("SpecificPosition")]
    protected abstract ObjectList<AccessControlEntry> AccessControlEntries { get; }

    public override string DisplayName
    {
      get { return Name; }
    }

    protected override void OnDeleting (EventArgs args)
    {
      base.OnDeleting (args);

      _deleteHandler = new DomainObjectDeleteHandler (AccessControlEntries, Roles, GroupTypes);
    }

    protected override void OnDeleted (EventArgs args)
    {
      base.OnDeleted (args);

      _deleteHandler.Delete();
    }

    protected override IDictionary<string, Enum> GetStates ()
    {
      IDictionary<string, Enum> states = base.GetStates ();
      states.Add ("Delegation", Delegation);

      return states;
    }
  }
}
