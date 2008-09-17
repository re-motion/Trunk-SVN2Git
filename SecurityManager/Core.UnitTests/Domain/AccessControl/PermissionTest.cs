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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class PermissionTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedTrue ()
    {
      Permission permission = Permission.NewObject();
      permission.Allowed = true;

      Assert.IsTrue (permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedFalse ()
    {
      Permission permission = Permission.NewObject();
      permission.Allowed = false;

      Assert.IsFalse (permission.BinaryAllowed);
    }

    [Test]
    public void GetBinaryAllowed_WithAllowedNull ()
    {
      Permission permission = Permission.NewObject();
      permission.Allowed = null;

      Assert.IsFalse (permission.BinaryAllowed);
    }

    [Test]
    public void SetBinaryAllowed_FromTrue()
    {
      Permission permission = Permission.NewObject();
      permission.BinaryAllowed = true;

      Assert.AreEqual (true, permission.Allowed);
    }

    [Test]
    public void SetBinaryAllowed_FromFalse ()
    {
      Permission permission = Permission.NewObject();
      permission.BinaryAllowed = false;

      Assert.IsNull (permission.Allowed);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      Permission permission = Permission.NewObject();

      permission.Index = 1;
      Assert.AreEqual (1, permission.Index);
    }
  }
}
