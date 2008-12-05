// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
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

    private ObjectList<AccessControlEntry> _accessControlEntriesToBeDeleted;
    private ObjectList<Role> _rolesToBeDeleted;
    private ObjectList<GroupTypePosition> _groupTypePositionsToBeDeleted;

    protected Position ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

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

      _accessControlEntriesToBeDeleted = AccessControlEntries.Clone ();
      _rolesToBeDeleted = Roles.Clone ();
      _groupTypePositionsToBeDeleted = GroupTypes.Clone ();
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

      foreach (GroupTypePosition groupTypePosition in _groupTypePositionsToBeDeleted)
        groupTypePosition.Delete ();
      _groupTypePositionsToBeDeleted = null;
    }

    protected override IDictionary<string, Enum> GetStates ()
    {
      IDictionary<string, Enum> states = base.GetStates ();
      states.Add ("Delegation", Delegation);

      return states;
    }
  }
}
