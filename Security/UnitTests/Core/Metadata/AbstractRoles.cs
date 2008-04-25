using System;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  public static class AbstractRoles
  {
    public static readonly EnumValueInfo Clerk = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Clerk", 0);
    public static readonly EnumValueInfo Secretary = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Secretary", 1);
    public static readonly EnumValueInfo Administrator = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.SpecialAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Administrator", 0);
  }
}