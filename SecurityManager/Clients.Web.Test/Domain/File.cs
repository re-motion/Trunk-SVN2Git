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
  [PermanentGuid ("BAA77408-32E6-4979-9914-8A12B71808F2")]
  [Instantiable]
  [DBTable]
  [DBStorageGroup]
  public abstract class File : BaseSecurableObject
  {
    public enum Method
    {
      CreateFileItem
    }

    public static File NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return DomainObject.NewObject<File> ().With ();
      }
    }

    public static File GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return DomainObject.GetObject<File> (id);
      }
    }

    protected File ()
    {
    }

    [Mandatory]
    public abstract Tenant Tenant { get; set; }

    //[DemandPropertyGetterPermission (DomainAccessTypes.ReadName)]
    //[DemandPropertySetterPermission (DomainAccessTypes.WriteName)]
    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [PermanentGuid ("4B073E2B-C56D-419c-8358-808FDEF669EF")]
    public abstract Confidentiality Confidentiality { get; set; }

    [Mandatory]
    public abstract User Creator { get; set; }

    public abstract User Clerk { get; set; }

    public abstract Group Group { get; set; }

    [DBBidirectionalRelation ("File")]
    public abstract ObjectList<FileItem> Files { get; set; }

    public override User GetOwner ()
    {
      return Clerk;
    }

    public override Group GetOwnerGroup ()
    {
      if (Clerk == null)
        return null;
      return Clerk.OwningGroup;
    }

    public override Tenant GetOwnerTenant ()
    {
      return Tenant;
    }
  }
}
