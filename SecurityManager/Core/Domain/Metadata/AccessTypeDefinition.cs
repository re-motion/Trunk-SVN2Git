// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
