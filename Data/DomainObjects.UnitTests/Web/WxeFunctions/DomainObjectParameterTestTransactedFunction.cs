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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Web.ExecutionEngine;
using NUnit.Framework;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class DomainObjectParameterTestTransactedFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectParameterTestTransactedFunction (
        WxeTransactionMode transactionMode,
        ClassWithAllDataTypes inParameter,
        ClassWithAllDataTypes[] inParameterArray)
        : base (transactionMode, inParameter, inParameterArray)
    {
    }

    // methods and properties

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public ClassWithAllDataTypes InParameter
    {
      get { return (ClassWithAllDataTypes) Variables["InParameter"]; }
      set { Variables["InParameter"] = value; }
    }

    [WxeParameter (2, false, WxeParameterDirection.In)]
    public ClassWithAllDataTypes[] InParameterArray
    {
      get { return (ClassWithAllDataTypes[]) Variables["InParameterArray"]; }
      set { Variables["InParameterArray"] = value; }
    }

    [WxeParameter (3, false, WxeParameterDirection.Out)]
    public ClassWithAllDataTypes OutParameter
    {
      get { return (ClassWithAllDataTypes) Variables["OutParameter"]; }
      set { Variables["OutParameter"] = value; }
    }

    [WxeParameter (4, false, WxeParameterDirection.Out)]
    public ClassWithAllDataTypes[] OutParameterArray
    {
      get { return (ClassWithAllDataTypes[]) Variables["OutParameterArray"]; }
      set { Variables["OutParameterArray"] = value; }
    }

    private void Step1 ()
    {
      Assert.IsTrue (Transaction == null || Transaction == ClientTransactionScope.CurrentTransaction);
      Assert.IsTrue (InParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.IsTrue (InParameterArray[0].CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));

      OutParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes1);
      OutParameter.Int32Property = InParameter.Int32Property + 5;

      OutParameterArray = new ClassWithAllDataTypes[] {ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes2)};
      OutParameterArray[0].Int32Property = InParameterArray[0].Int32Property + 5;
    }
  }
}
