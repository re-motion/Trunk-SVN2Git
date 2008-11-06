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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Permission : AccessControlObject
  {
    public static Permission NewObject ()
    {
      return NewObject<Permission> ().With ();
    }

    protected Permission ()
    {
    }

    public abstract int Index { get; set; }

    public abstract bool? Allowed { get; set; }

    [DBBidirectionalRelation ("Permissions")]
    [DBColumn ("AccessTypeDefinitionID")]
    [Mandatory]
    public abstract AccessTypeDefinition AccessType { get; set; }

    [DBBidirectionalRelation ("Permissions")]
    [Mandatory]
    public abstract AccessControlEntry AccessControlEntry { get; }
  }
}
