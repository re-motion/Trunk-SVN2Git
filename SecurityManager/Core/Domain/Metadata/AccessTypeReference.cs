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
using Remotion.Data.DomainObjects;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class AccessTypeReference : BaseSecurityManagerObject
  {
    public static AccessTypeReference NewObject ()
    {
      return NewObject<AccessTypeReference> ().With ();
    }

    protected AccessTypeReference ()
    {
    }

    public abstract int Index { get; set; }

    [DBBidirectionalRelation ("AccessTypeReferences")]
    [DBColumn ("SecurableClassID")]
    [Mandatory]
    public abstract SecurableClassDefinition Class { get; set; }

    [DBBidirectionalRelation ("References")]
    [Mandatory]
    public abstract AccessTypeDefinition AccessType { get; set; }
  }
}
