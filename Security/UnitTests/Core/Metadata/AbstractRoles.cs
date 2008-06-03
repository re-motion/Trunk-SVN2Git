/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
