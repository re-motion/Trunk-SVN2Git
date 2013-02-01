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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  [Ignore ("TODO 4405")]
  public class SecuredFunctionViaDomainObjectHandleParameterTest : SecuredFunctionTestBase
  {
    private ClientTransaction _clientTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectHandleParameter_WithObjectHasAccessTrue_Succeeds ()
    {
      var wxeFunction = CreateWxeFunction (_clientTransaction);
      ObjectSecurityStrategyStub.Stub (stub => stub.HasAccess (SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue)).Return (true);

      wxeFunction.Execute (Context);
    }

    [Test]
    public void ExecuteWithSecurityCheck_ViaDomainObjectHandleParameter_WithObjectHasAccessFalse_Fails ()
    {
      var wxeFunction = CreateWxeFunction (_clientTransaction);
      ObjectSecurityStrategyStub.Stub (stub => stub.HasAccess (SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue)).Return (false);

      Assert.That (() => wxeFunction.Execute (Context), Throws.TypeOf<WxeUnhandledException>().With.InnerException.TypeOf<PermissionDeniedException>());
    }

    [Test]
    public void HasAccess_ViaDomainObjectHandleParameter_WithFunctionalHasAccessTrue_ReturnsTrue ()
    {
      FunctionalSecurityStrategyStub
          .Stub (stub => stub.HasAccess (typeof (SecurableDomainObject), SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue))
          .Return (true);

      Assert.That (WxeFunction.HasAccess (typeof (FunctionWithSecuredDomainObjectHandleParameter)), Is.True);
    }

    [Test]
    public void HasAccess_ViaDomainObjectHandleParameter_WithFunctionalHasAccessFalse_ReturnsFalse ()
    {
      FunctionalSecurityStrategyStub
          .Stub (stub => stub.HasAccess (typeof (SecurableDomainObject), SecurityProviderStub, SecurityPrincipalStub, TestAccessTypeValue))
          .Return (false);

      Assert.That (WxeFunction.HasAccess (typeof (FunctionWithSecuredDomainObjectHandleParameter)), Is.False);
    }
    
    private FunctionWithSecuredDomainObjectHandleParameter CreateWxeFunction (ClientTransaction clientTransaction)
    {
      var securableDomainObject = CreateSecurableDomainObject (clientTransaction);

      var mode = MockRepository.GenerateStub<ITransactionMode>();
      mode.Stub (stub => stub.CreateTransactionStrategy (Arg<WxeFunction>.Is.Anything, Arg<WxeContext>.Is.Anything))
          .Do (
              (Func<WxeFunction, WxeContext, TransactionStrategyBase>)
              ((function, context) => new RootTransactionStrategy (false, clientTransaction.ToITransaction, NullTransactionStrategy.Null, function)));

      var wxeFunction = new FunctionWithSecuredDomainObjectHandleParameter (mode);
      wxeFunction.SecurableParameter = securableDomainObject.GetHandle();
      return wxeFunction;
    }
  }
}