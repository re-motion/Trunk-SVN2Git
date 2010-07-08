// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class HasMethodAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private MethodInfo _methodInfo;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatelessSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
      _methodInfo = typeof (SecurableObject).GetMethod ("IsValid", new[] { typeof (SecurableObject) });
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, "InstanceMethod");

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_AccessGranted_WithMethodInfo ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, "InstanceMethod");
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted_WithMethodInfo ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInfo);
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (null), "InstanceMethod");
    
      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull_WithMethodInfo ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasMethodAccess (new SecurableObject (null), _methodInfo);

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}
