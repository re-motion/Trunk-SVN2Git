using System;

namespace Remotion.Security.UnitTests.TestDomain
{
  [AbstractRole]
  public enum DomainAbstractRoles
  {
    [PermanentGuid ("00000003-0001-0000-0000-000000000000")]
    Clerk,
    [PermanentGuid ("00000003-0002-0000-0000-000000000000")]
    Secretary
  }
}