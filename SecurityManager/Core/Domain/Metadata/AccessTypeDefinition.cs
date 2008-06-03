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
