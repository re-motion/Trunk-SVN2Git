using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security
{
  public interface IDomainObjectSecurityContextFactory : ISecurityContextFactory
  {
    bool IsDiscarded { get; }
    bool IsNew { get; }
    bool IsDeleted { get; }
  }
}