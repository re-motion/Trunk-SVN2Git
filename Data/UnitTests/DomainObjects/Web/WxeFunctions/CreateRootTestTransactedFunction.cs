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
  public class CreateRootTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public CreateRootTestTransactedFunction (ClientTransactionScope previousClientTransactionScope)
        : base (WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit, previousClientTransactionScope)
    {
    }

    // methods and properties

    [WxeParameter (1, true, WxeParameterDirection.In)]
    public ClientTransactionScope PreviousClientTransactionScope
    {
      get { return (ClientTransactionScope) Variables["PreviousClientTransactionScope"]; }
      set { Variables["PreviousClientTransactionScope"] = value; }
    }

    private void Step1 ()
    {
      Assert.AreNotSame (PreviousClientTransactionScope, ClientTransactionScope.ActiveScope);
    }
  }
}
