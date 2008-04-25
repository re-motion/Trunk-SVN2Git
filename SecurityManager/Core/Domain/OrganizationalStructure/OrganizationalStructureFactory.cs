using System;
using Remotion.Data.DomainObjects;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  public class OrganizationalStructureFactory : IOrganizationalStructureFactory
  {
    public virtual Tenant CreateTenant ()
    {
      return Tenant.NewObject ();
    }

    public virtual Group CreateGroup ()
    {
      return Group.NewObject ();
    }

    public virtual User CreateUser ()
    {
      return User.NewObject ();
    }

    public virtual Position CreatePosition ()
    {
      return Position.NewObject ();
    }

    public virtual Type GetTenantType ()
    {
      return typeof (Tenant);
    }

    public virtual Type GetGroupType ()
    {
      return typeof (Group);
    }

    public virtual Type GetUserType ()
    {
      return typeof (User);
    }

    public virtual Type GetPositionType ()
    {
      return typeof (Position);
    }
  }
}
