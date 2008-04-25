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
