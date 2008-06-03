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
