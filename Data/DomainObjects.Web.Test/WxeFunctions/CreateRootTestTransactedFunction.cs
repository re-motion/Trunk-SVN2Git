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
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  [Serializable]
  public class CreateRootTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CreateRootTestTransactedFunction (ClientTransaction previousClientTransaction)
        : base (WxeTransactionMode.CreateRootWithAutoCommit, previousClientTransaction)
    {
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ClientTransaction PreviousClientTransaction
    {
      get { return (ClientTransaction) Variables["PreviousClientTransaction"]; }
      set { Variables["PreviousClientTransaction"] = value; }
    }

    private void Step1 ()
    {
      if (ClientTransactionScope.CurrentTransaction == PreviousClientTransaction)
        throw new TestFailureException ("The WxeTransactedFunction did not properly set a new ClientTransaction.");
    }
  }
}
