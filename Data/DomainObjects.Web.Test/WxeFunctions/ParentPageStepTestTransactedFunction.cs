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

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  using WxeTransactedFunction = Remotion.Web.ExecutionEngine.WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  public class ParentPageStepTestTransactedFunction : WxeTransactedFunction
  {
    private ClientTransaction _transactionInStep1;

    private void Step1 ()
    {
      _transactionInStep1 = ClientTransactionScope.CurrentTransaction;
    }

    private NestedPageStepTestTransactedFunction Step2 = new NestedPageStepTestTransactedFunction ();

    private void Step3 ()
    {
      if (_transactionInStep1 != ClientTransactionScope.CurrentTransaction)
        throw new TestFailureException ("Transaction in parent function was not restored.");
    }
  }
}
