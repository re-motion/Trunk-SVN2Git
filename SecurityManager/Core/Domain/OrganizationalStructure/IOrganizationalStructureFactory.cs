using System;
using Remotion.Data.DomainObjects;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  public interface IOrganizationalStructureFactory
  {
    Tenant CreateTenant ();
    Group CreateGroup ();
    User CreateUser ();
    Position CreatePosition ();
    Type GetTenantType ();
    Type GetGroupType ();
    Type GetUserType ();
    Type GetPositionType ();

  }
}
