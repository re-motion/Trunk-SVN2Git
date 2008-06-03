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
using System.Security.Principal;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class NullIdentityTest
  {
    private IIdentity _identity;

    [SetUp]
    public void SetUp()
    {
      _identity = new NullIdentity();
    }

    [Test]
    public void GetName()
    {
      Assert.AreEqual (string.Empty, _identity.Name);
    }

    [Test]
    public void GetIsAuthenticated()
    {
      Assert.IsFalse (_identity.IsAuthenticated);
    }

    [Test]
    public void GetAuthenticationType()
    {
      Assert.AreEqual (string.Empty, _identity.AuthenticationType);
    }
  }
}
