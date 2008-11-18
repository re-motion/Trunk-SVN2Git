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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryValidationResultTest
  {
    [Test]
    public void IsValid_Valid ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      Assert.That (result.IsValid, Is.True);
      Assert.That (result.GetErrors(), Is.Empty);
    }

    [Test]
    public void IsValid_NotValid ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError (AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That (result.IsValid, Is.False);
    }

    [Test]
    public void SetError ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError (AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That (result.GetErrors(), Is.EquivalentTo (new[] { AccessControlEntryValidationError.IsSpecificTenantMissing }));
    }

    [Test]
    public void SetError_SameTwice ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError (AccessControlEntryValidationError.IsSpecificTenantMissing);
      result.SetError (AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That (result.GetErrors(), Is.EquivalentTo (new[] { AccessControlEntryValidationError.IsSpecificTenantMissing }));
    }

    [Test]
    public void SetError_DifferentValues ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError (AccessControlEntryValidationError.IsSpecificTenantMissing);
      result.SetError (AccessControlEntryValidationError.IsSpecificGroupMissing);

      Assert.That (
          result.GetErrors(),
          Is.EquivalentTo (
              new[] { AccessControlEntryValidationError.IsSpecificTenantMissing, AccessControlEntryValidationError.IsSpecificGroupMissing }));
    }

    [Test]
    public void GetErrors ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError (AccessControlEntryValidationError.IsSpecificGroupMissing);
      result.SetError (AccessControlEntryValidationError.IsSpecificTenantMissing);

      Assert.That (
          result.GetErrors(),
          Is.EqualTo (new[] { AccessControlEntryValidationError.IsSpecificTenantMissing, AccessControlEntryValidationError.IsSpecificGroupMissing }));
    }

    [Test]
    public void GetErrorMessage ()
    {
      AccessControlEntryValidationResult result = new AccessControlEntryValidationResult();

      result.SetError (AccessControlEntryValidationError.IsSpecificGroupMissing);
      result.SetError (AccessControlEntryValidationError.IsSpecificTenantMissing);

      using (new CultureScope (""))
      {
        Assert.That (
            result.GetErrorMessage(),
            Is.EqualTo (
                "The access control entry is in an invalid state:\r\n"
                + "  The TenantCondition property is set to SpecificTenant, but no SpecificTenant is assigned.\r\n"
                + "  The GroupCondition property is set to SpecificGroup, but no SpecificGroup is assigned."));
      }
    }
  }
}