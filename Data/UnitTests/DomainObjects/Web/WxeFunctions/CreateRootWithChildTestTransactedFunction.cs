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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  [Serializable]
  public class CreateRootWithChildTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CreateRootWithChildTestTransactedFunction (ClientTransaction previousClientTransaction, WxeFunction childFunction)
        : base (WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit, childFunction, previousClientTransaction)
    {
      Add (
          new WxeMethodStep (
              delegate ()
              {
                TransactionAfterChild = ClientTransactionScope.CurrentTransaction;
                Assert.AreSame (TransactionBeforeChild, TransactionAfterChild);
              }));
    }

    public ClientTransaction TransactionBeforeChild;
    public ClientTransaction TransactionAfterChild;

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ClientTransaction PreviousClientTransaction
    {
      get { return (ClientTransaction) Variables["PreviousClientTransaction"]; }
      set { Variables["PreviousClientTransaction"] = value; }
    }

    private void Step1 ()
    {
      Assert.AreNotSame (PreviousClientTransaction, ClientTransactionScope.CurrentTransaction);
      TransactionBeforeChild = ClientTransactionScope.CurrentTransaction;
    }
  }
}
