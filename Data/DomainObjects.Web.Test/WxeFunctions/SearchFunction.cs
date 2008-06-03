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
using System.Collections;
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Mixins;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class SearchFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SearchFunction ()
    {
      ReturnUrl = "default.aspx";
    }

    // methods and properties

    public ClassWithAllDataTypesSearch SearchObject
    {
      get { return (ClassWithAllDataTypesSearch) Variables["SearchObject"]; }
      set { Variables["SearchObject"] = value; }
    }

    public IList Result
    {
      get { return (IList) Variables["Result"]; }
      set { Variables["Result"] = value; }
    }

    public void Requery ()
    {
      Result = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (SearchObject.CreateQuery());
    }

    private void Step1 ()
    {
      SearchObject = ObjectFactory.Create<ClassWithAllDataTypesSearch>().With();
      Requery();
    }

    private WxePageStep Step2 = new WxePageStep ("SearchObject.aspx");
  }
}
