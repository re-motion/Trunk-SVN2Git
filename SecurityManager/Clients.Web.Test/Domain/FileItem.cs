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
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.Clients.Web.Test.Domain
{
  [PermanentGuid ("DF0A8DB4-943C-4bd1-8B3B-276C8AA16BDB")]
  [Instantiable]
  [DBTable]
  [DBStorageGroup]
  public abstract class FileItem : BaseSecurableObject
  {
    public static FileItem NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return DomainObject.NewObject<FileItem> ().With ();
      }
    }

    public static FileItem GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return DomainObject.GetObject<FileItem> (id);
      }
    }

    protected FileItem ()
    {
    }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Files")]
    [Mandatory]
    public abstract File File { get; set; }

    public override User GetOwner ()
    {
      if (File == null)
        return null;
      return File.GetOwner();
    }

    public override Group GetOwnerGroup ()
    {
      if (File == null)
        return null;
      return File.GetOwnerGroup ();
    }

    public override Tenant GetOwnerTenant ()
    {
      if (File == null)
        return null;
      return File.GetOwnerTenant ();
    }
  }
}
