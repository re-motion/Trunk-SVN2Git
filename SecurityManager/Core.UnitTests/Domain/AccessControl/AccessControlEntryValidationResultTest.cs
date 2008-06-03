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
using NUnit.Framework;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryValidationResultTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult ();

      Assert.IsTrue (result.IsValid);
      Assert.IsFalse (result.IsSpecificTenantMissing);
    }

    [Test]
    public void IsValid_IsSpecificTenantMissing ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult ();

      result.SetSpecificTenantMissing ();

      Assert.IsFalse (result.IsValid);
      Assert.IsTrue (result.IsSpecificTenantMissing);
    }

  }
}
