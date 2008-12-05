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
using Remotion.Data.DomainObjects;
using Remotion.Globalization;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.GroupTypePosition")]
  [PermanentGuid ("E2BF5572-DDFF-4319-8824-B41653950860")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class GroupTypePosition : OrganizationalStructureObject
  {
    public static GroupTypePosition NewObject ()
    {
      return NewObject<GroupTypePosition> ().With ();
    }

    protected GroupTypePosition ()
    {
    }

    [DBBidirectionalRelation ("Positions")]
    [Mandatory]
    public abstract GroupType GroupType { get; set; }

    [DBBidirectionalRelation ("GroupTypes")]
    [Mandatory]
    public abstract Position Position { get; set; }

    public override string DisplayName
    {
      get
      {
        string groupTypeName = (GroupType != null) ? GroupType.Name : string.Empty;
        string positionName = (Position != null) ? Position.Name : string.Empty;

        return string.Format ("{0} / {1}", groupTypeName, positionName); 
      }
    }
  }
}
