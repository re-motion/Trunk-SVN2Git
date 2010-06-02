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
using System.Security;
using System.Security.Policy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Sandboxing;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class SandboxTest
  {
    private IPermission[] _mediumTrustPermissions;

    [SetUp]
    public void SetUp ()
    {
      _mediumTrustPermissions = PermissionSets.GetMediumTrust (AppDomain.CurrentDomain.BaseDirectory, Environment.MachineName);
    }

    [Test]
    public void CreateSandbox ()
    {
      var currentAppDomain = AppDomain.CurrentDomain;
      using (var sandbox = Sandbox.CreateSandbox (_mediumTrustPermissions, new Assembly[0]))
      {
        Assert.That (sandbox.AppDomain, Is.Not.Null);
        Assert.That (sandbox.AppDomain, Is.Not.SameAs (currentAppDomain));
        Assert.That (sandbox.AppDomain.FriendlyName.StartsWith ("Sandbox ("), Is.True);
      }
    }

    [Test]
    [ExpectedException (typeof (AppDomainUnloadedException))]
    public void Dispose ()
    {
      var sandbox = Sandbox.CreateSandbox (_mediumTrustPermissions, new Assembly[0]);
      sandbox.Dispose();
      sandbox.CreateSandboxedInstance<SampleTestRunner> (_mediumTrustPermissions);
    }

    [Test]
    public void Dispose_Twice ()
    {
      var sandbox = Sandbox.CreateSandbox (_mediumTrustPermissions, new Assembly[0]);
      sandbox.Dispose ();
      sandbox.Dispose ();
    }

    [Test]
    public void CreateSandboxedInstance ()
    {
      using (var sandbox = Sandbox.CreateSandbox (_mediumTrustPermissions, new Assembly[0]))
      {
        var result = sandbox.CreateSandboxedInstance<SampleTestRunner>();
        Assert.That (result, Is.TypeOf (typeof (SampleTestRunner)));
      }
    }

    class SampleTestRunner : MarshalByRefObject
    {
    }
  }
}