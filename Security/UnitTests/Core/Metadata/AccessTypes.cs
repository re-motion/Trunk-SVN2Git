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

  public static class AccessTypes
  {
    public static readonly EnumValueInfo Read = new EnumValueInfo ("Security.GeneralAccessTypes, Remotion", "Read", 0);
    public static readonly EnumValueInfo Write = new EnumValueInfo ("Security.Security.GeneralAccessTypes, Remotion", "Write", 1);
    public static readonly EnumValueInfo Journalize = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain", "Journalize", 0);
    public static readonly EnumValueInfo Archive = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain", "Archive", 1);
  }
}
