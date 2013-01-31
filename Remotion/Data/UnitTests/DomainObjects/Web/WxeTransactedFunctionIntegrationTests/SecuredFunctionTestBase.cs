﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Security.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  public class SecuredFunctionTestBase : WxeTransactedFunctionIntegrationTestBase
  {
    private IWxeSecurityAdapter _previousAdapter;

    private ISecurityProvider _securityProviderStub;
    private ISecurityPrincipal _securityPrincipalStub;
    private IFunctionalSecurityStrategy _functionalSecurityStrategyStub;
    private IObjectSecurityStrategy _objectSecurityStrategyStub;
    private AccessType _testAccessTypeValue;

    public override void SetUp ()
    {
      base.SetUp ();

      _previousAdapter = AdapterRegistry.Instance.GetAdapter<IWxeSecurityAdapter> ();
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), new WxeSecurityAdapter ());

      _securityProviderStub = MockRepository.GenerateStub<ISecurityProvider> ();
      _securityProviderStub.Stub (stub => stub.IsNull).Return (false);

      _securityPrincipalStub = MockRepository.GenerateStub<ISecurityPrincipal> ();
      var principalProviderStub = MockRepository.GenerateStub<IPrincipalProvider> ();
      principalProviderStub.Stub (stub => stub.GetPrincipal ()).Return (_securityPrincipalStub);

      _functionalSecurityStrategyStub = MockRepository.GenerateStub<IFunctionalSecurityStrategy> ();
      _objectSecurityStrategyStub = MockRepository.GenerateStub<IObjectSecurityStrategy> ();

      SecurityConfiguration.Current.SecurityProvider = _securityProviderStub;
      SecurityConfiguration.Current.PrincipalProvider = principalProviderStub;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _functionalSecurityStrategyStub;

      _testAccessTypeValue = AccessType.Get (TestAccessTypes.Value);
    }

    public override void TearDown ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _previousAdapter);
      SecurityConfiguration.Current.SecurityProvider = null;
      SecurityConfiguration.Current.PrincipalProvider = null;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = null;

      base.TearDown ();
    }

    protected ISecurityProvider SecurityProviderStub
    {
      get { return _securityProviderStub; }
    }

    protected ISecurityPrincipal SecurityPrincipalStub
    {
      get { return _securityPrincipalStub; }
    }

    protected IFunctionalSecurityStrategy FunctionalSecurityStrategyStub
    {
      get { return _functionalSecurityStrategyStub; }
    }

    protected IObjectSecurityStrategy ObjectSecurityStrategyStub
    {
      get { return _objectSecurityStrategyStub; }
    }

    protected AccessType TestAccessTypeValue
    {
      get { return _testAccessTypeValue; }
    }

    protected SecurableDomainObject CreateSecurableDomainObject ()
    {
      var securableDomainObject = DomainObjectMother.CreateFakeObject<SecurableDomainObject>();
      securableDomainObject.SecurableType = typeof (SecurableDomainObject);
      securableDomainObject.SecurityStrategy = _objectSecurityStrategyStub;
      return securableDomainObject;
    }
  }
}