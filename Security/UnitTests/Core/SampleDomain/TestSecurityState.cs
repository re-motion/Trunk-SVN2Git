using System;

namespace Remotion.Security.UnitTests.Core.SampleDomain
{
  [SecurityState]
  public enum TestSecurityState
  {
    Public,
    Confidential,
    Secret
  }
}
