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
  public class NullPrincipalTest
  {
    private IPrincipal _principal;

    [SetUp]
    public void SetUp()
    {
      _principal = new NullPrincipal();
    }

    [Test]
    public void IsInRole()
    {
      Assert.IsFalse (_principal.IsInRole (string.Empty));
    }

    [Test]
    public void GetIdentity()
    {
      Assert.IsInstanceOfType (typeof (NullIdentity), _principal.Identity);
    }
  }
}
