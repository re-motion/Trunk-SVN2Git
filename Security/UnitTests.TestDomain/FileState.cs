using System;

namespace Remotion.Security.UnitTests.TestDomain
{
  [SecurityState]
  public enum FileState
  {
    New = 0,
    Normal = 1,
    Archived = 2
  }
}