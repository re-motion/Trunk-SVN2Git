using System;
using System.Collections.Generic;
using System.Text;

using Remotion.Utilities;

namespace Remotion.Security.Data.DomainObjects
{
  public interface IDomainObjectSecurityContextFactory : ISecurityContextFactory
  {
    bool IsDiscarded { get; }
    bool IsNew { get; }
    bool IsDeleted { get; }
  }
}