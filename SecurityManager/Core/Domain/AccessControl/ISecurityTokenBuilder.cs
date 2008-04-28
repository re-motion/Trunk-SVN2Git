using System;
using System.Security.Principal;
using Remotion.Data.DomainObjects;
using Remotion.Security;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public interface ISecurityTokenBuilder
  {
    SecurityToken CreateToken (ClientTransaction transaction, IPrincipal user, ISecurityContext context);
  }
}
