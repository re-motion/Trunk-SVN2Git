using System;
using Remotion.Security;

namespace Remotion.SecurityManager.UnitTests.TestDomain
{
  [AbstractRole]
  public enum ProjectRoles
  {
    QualityManager,
    Developer
  }

  [AbstractRole]
  public enum UndefinedAbstractRoles
  {
    Undefined
  }
}
