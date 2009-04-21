// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
