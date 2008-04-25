using System;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata
{
  public static class PropertyStates
  {
    public static readonly EnumValueInfo FileStateNew = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain", "New", 0);
    public static readonly EnumValueInfo FileStateNormal = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain", "Normal", 1);
    public static readonly EnumValueInfo FileStateArchived = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain", "Archived", 2);
    public static readonly EnumValueInfo ConfidentialityNormal = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain", "Normal", 0);
    public static readonly EnumValueInfo ConfidentialityConfidential = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain", "Confidential", 1);
    public static readonly EnumValueInfo ConfidentialityPrivate = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain", "Private", 2);
  }
}