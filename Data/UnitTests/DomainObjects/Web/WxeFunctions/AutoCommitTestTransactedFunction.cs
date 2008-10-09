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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  [Serializable]
  public class AutoCommitTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public AutoCommitTestTransactedFunction (ITransactionMode transactionMode, ObjectID objectWithAllDataTypes)
        : base (transactionMode, objectWithAllDataTypes)
    {
      Assertion.IsTrue (TransactionMode.AutoCommit);
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ObjectID ObjectWithAllDataTypes
    {
      get { return (ObjectID) Variables["ObjectWithAllDataTypes"]; }
      set { Variables["ObjectWithAllDataTypes"] = value; }
    }

    private void Step1 ()
    {
      ClassWithAllDataTypes objectWithAllDataTypes = ClassWithAllDataTypes.GetObject (ObjectWithAllDataTypes);

      objectWithAllDataTypes.Int32Property = 10;
    }
  }
}
