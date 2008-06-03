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

namespace Remotion.Security.UnitTests.TestDomain
{
  [AbstractRole]
  public enum SpecialAbstractRoles
  {
    [PermanentGuid ("00000004-0001-0000-0000-000000000000")]
    Administrator,
  }
}
