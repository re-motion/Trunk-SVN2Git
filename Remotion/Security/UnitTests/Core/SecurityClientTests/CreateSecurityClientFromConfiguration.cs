// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Security.Configuration;
using Remotion.Security.UnitTests.Core.Configuration;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core.SecurityClientTests
{
  [TestFixture]
  public class CreateSecurityClientFromConfiguration
  {
    private MockRepository _mocks;
    private SecurityConfiguration _configuration;
    private ISecurityProvider _provider;

    [SetUp]
    public void SetUp()
    {
      _mocks = new MockRepository();
      _configuration = new SecurityConfiguration ();
      SecurityConfigurationMock.SetCurrent (_configuration);
      _provider = _mocks.StrictMock<ISecurityProvider>();
      _configuration.SecurityProvider = _provider;
    }

    [TearDown]
    public void TearDown()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void CreateNormal()
    {
      Expect.Call (_provider.IsNull).Return (false);
      _mocks.ReplayAll();
      
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      
      _mocks.VerifyAll();
      Assert.IsInstanceOf (typeof (SecurityClient), securityClient);
      Assert.That (((INullObject) securityClient).IsNull, Is.False);
    }

    [Test]
    public void CreateNull ()
    {
      Expect.Call (_provider.IsNull).Return (true);
      _mocks.ReplayAll ();

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();

      _mocks.VerifyAll ();
      Assert.IsInstanceOf (typeof (NullSecurityClient), securityClient);
      Assert.That (((INullObject) securityClient).IsNull, Is.True);
    }
  }
}
