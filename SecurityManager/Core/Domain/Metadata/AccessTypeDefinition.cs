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
using System.ComponentModel;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  public abstract class AccessTypeDefinition : EnumValueDefinition
  {
    public static AccessTypeDefinition NewObject ()
    {
      return NewObject<AccessTypeDefinition> ().With ();
    }

    public static AccessTypeDefinition NewObject (Guid metadataItemID, string name, int value)
    {
      return NewObject<AccessTypeDefinition> ().With (metadataItemID, name, value);
    }

    protected AccessTypeDefinition ()
    {
    }

    protected AccessTypeDefinition (Guid metadataItemID, string name, int value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      MetadataItemID = metadataItemID;
      Name = name;
      Value = value;
    }

    [DBBidirectionalRelation ("AccessType")]
    public abstract ObjectList<AccessTypeReference> References { get; }

    [DBBidirectionalRelation ("AccessType")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected abstract ObjectList<Permission> Permissions { get; }
  }
}
