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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class SecuredFunctionViaDomainObjectParameterTest : WxeTransactedFunctionIntegrationTestBase
  {
    private IWxeSecurityAdapter _previousAdapter;

    private ISecurityProvider _securityProviderMock;
    private ISecurityPrincipal _securityPrincipalStub;
    private IFunctionalSecurityStrategy _functionalSecurityStrategyMock;

    private IObjectSecurityStrategy _objectSecurityStrategyStub;
    private AccessType _testAccessTypeValue;

    public override void SetUp ()
    {
      base.SetUp ();

      _previousAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter>();
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), new WxeSecurityAdapter ());

      _securityProviderMock = MockRepository.GenerateStrictMock<ISecurityProvider> ();
      _securityProviderMock.Stub (stub => stub.IsNull).Return (false);

      var _principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider> ();
      _securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      _functionalSecurityStrategyMock = MockRepository.GenerateStrictMock<IFunctionalSecurityStrategy> ();

      _principalProviderStub.Stub (stub => stub.GetPrincipal ()).Return (_securityPrincipalStub);

      SecurityConfiguration.Current.SecurityProvider = _securityProviderMock;
      SecurityConfiguration.Current.PrincipalProvider = _principalProviderStub;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _functionalSecurityStrategyMock;

      _objectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy> ();
      _testAccessTypeValue = AccessType.Get (FunctionWithSecuredDomainObjectParameter.TestAccessTypes.Value);
    }

    public override void TearDown ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _previousAdapter);

      base.TearDown ();
    }

    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectParameter_WithObjectHasAccessTrue_Succeeds ()
    {
      var wxeFunction = CreateWxeFunctionWithSecurityOnDomainObject();
      _objectSecurityStrategyStub.Stub (stub => stub.HasAccess (_securityProviderMock, _securityPrincipalStub, _testAccessTypeValue)).Return (true);

      wxeFunction.Execute (Context);
    }

    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectParameter_WithObjectHasAccessFalse_Fails ()
    {
      var wxeFunction = CreateWxeFunctionWithSecurityOnDomainObject ();
      _objectSecurityStrategyStub.Stub (stub => stub.HasAccess (_securityProviderMock, _securityPrincipalStub, _testAccessTypeValue)).Return (false);

      Assert.That (() => wxeFunction.Execute (Context), Throws.TypeOf<WxeUnhandledException>().With.InnerException.TypeOf<PermissionDeniedException>());
    }

    [Test]
    public void HasAccess_ViaDomainObjectParameter_WithFunctionalHasAccessTrue_ReturnsTrue ()
    {
      _functionalSecurityStrategyMock
          .Stub (stub => stub.HasAccess (
              typeof (FunctionWithSecuredDomainObjectParameter.SecurableDomainObject),
              _securityProviderMock,
              _securityPrincipalStub,
              _testAccessTypeValue))
          .Return (true);

      Assert.That (WxeFunction.HasAccess (typeof (FunctionWithSecuredDomainObjectParameter)), Is.True);
    }

    [Test]
    public void HasAccess_ViaDomainObjectParameter_WithFunctionalHasAccessFalse_ReturnsFalse ()
    {
      _functionalSecurityStrategyMock
          .Stub (stub => stub.HasAccess (
              typeof (FunctionWithSecuredDomainObjectParameter.SecurableDomainObject),
              _securityProviderMock,
              _securityPrincipalStub,
              _testAccessTypeValue))
          .Return (false);

      Assert.That (WxeFunction.HasAccess (typeof (FunctionWithSecuredDomainObjectParameter)), Is.False);
    }
    
    private FunctionWithSecuredDomainObjectParameter CreateWxeFunctionWithSecurityOnDomainObject ()
    {
      var wxeFunction = new FunctionWithSecuredDomainObjectParameter (WxeTransactionMode<ClientTransactionFactory>.CreateRoot);
      wxeFunction.SecurableParameter = DomainObjectMother.CreateFakeObject<FunctionWithSecuredDomainObjectParameter.SecurableDomainObject>();
      wxeFunction.SecurableParameter.SecurableType = typeof (FunctionWithSecuredDomainObjectParameter.SecurableDomainObject);
      wxeFunction.SecurableParameter.SecurityStrategy = _objectSecurityStrategyStub;
      return wxeFunction;
    }
  }
}