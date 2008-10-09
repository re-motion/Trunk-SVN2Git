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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  [Serializable]
  public class DomainObjectParameterInvalidOutTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectParameterInvalidOutTestTransactedFunction (ITransactionMode transactionMode)
        : base (transactionMode)
    {
    }

    // methods and properties

    [WxeParameter (1, false, WxeParameterDirection.Out)]
    public ClassWithAllDataTypes OutParameter
    {
      get { return (ClassWithAllDataTypes) Variables["OutParameter"]; }
      set { Variables["OutParameter"] = value; }
    }

    private void Step1 ()
    {
      OutParameter = ClassWithAllDataTypes.NewObject();
      OutParameter.Delete();
    }
  }
}
