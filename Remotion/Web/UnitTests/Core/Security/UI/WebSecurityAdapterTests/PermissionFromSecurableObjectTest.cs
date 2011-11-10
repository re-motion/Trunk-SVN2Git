// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.Web.Security.UI;
using Remotion.Web.UnitTests.Core.Security.Configuration;
using Remotion.Web.UnitTests.Core.Security.Domain;
using Remotion.Web.UI;

namespace Remotion.Web.UnitTests.Core.Security.UI.WebSecurityAdapterTests
{
  [TestFixture]
  public class PermissionFromSecurableObjectTest
  {
    private IWebSecurityAdapter _securityAdapter;
    private WebPermissionProviderTestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WebSecurityAdapter ();

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
      _testHelper = new WebPermissionProviderTestHelper ();
      SecurityConfiguration.Current.SecurityProvider = _testHelper.SecurityProvider;
      SecurityConfiguration.Current.PrincipalProvider = _testHelper.PrincipalProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _testHelper.FunctionalSecurityStrategy;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void HasAccessGranted_WithoutHandler ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), null);

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessGranted_WithinSecurityFreeSection ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessGranted ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessTypes.Read }, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      _testHelper.ExpectHasAccess (new Enum[] { GeneralAccessTypes.Read }, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (_testHelper.CreateSecurableObject (), new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessGranted_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject (new Enum[] { GeneralAccessTypes.Read }, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject (new Enum[] { GeneralAccessTypes.Read }, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityAdapter.HasAccess (null, new EventHandler (TestEventHandler));

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetMethodPermission (SecurableObject.Method.Show)]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }
  }
}
