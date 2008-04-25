using System;

namespace Remotion.Security.UnitTests.TestDomain
{
  [AccessType]
  public enum DomainAccessTypes
  {
    [PermanentGuid ("00000002-0001-0000-0000-000000000000")]
    Journalize,
    [PermanentGuid ("00000002-0002-0000-0000-000000000000")]
    Archive
  }
}