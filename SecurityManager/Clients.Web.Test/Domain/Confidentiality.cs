using System;
using Remotion.Security;

namespace Remotion.SecurityManager.Clients.Web.Test.Domain
{
  [SecurityState]
  public enum Confidentiality
  {
    Normal = 0,
    Classified = 1,
    Secret = 2
  }
}
