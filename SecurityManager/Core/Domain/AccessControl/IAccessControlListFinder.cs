using System;
using Remotion.Data.DomainObjects;
using Remotion.Security;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public interface IAccessControlListFinder
  {
    AccessControlList Find (ClientTransaction transaction, SecurityContext context);
  }
}
