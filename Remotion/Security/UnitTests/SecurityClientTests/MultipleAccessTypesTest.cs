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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.SampleDomain;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class MultipleAccessTypesTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;
    private IMethodInformation _methodInformation;
    private MethodInfo _methodInfo;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTestHelper();
      _securityClient = _testHelper.CreateSecurityClient();
      _methodInformation = MockRepository.GenerateMock<IMethodInformation>();
      _methodInformation.Expect (n => n.Name).Return ("InstanceMethod");
      _methodInfo = typeof (SecurableObject).GetMethod ("IsValid", new[] { typeof (SecurableObject) });
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, GeneralAccessTypes.Edit, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (new Enum[] { GeneralAccessTypes.Edit, TestAccessTypes.First }, true);
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectMemberResolverGetMethodInformation (_methodInfo, MemberAffiliation.Instance, _methodInformation);
      _testHelper.ExpectPermissionReflectorGetRequiredMethodPermissions (_methodInformation, GeneralAccessTypes.Read, TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (new Enum[] { GeneralAccessTypes.Read, TestAccessTypes.First }, false);
      _testHelper.ReplayAll();

      bool hasAccess = _securityClient.HasMethodAccess (_testHelper.SecurableObject, _methodInfo);

      _testHelper.VerifyAll();
      Assert.That (hasAccess, Is.False);
    }
  }
}