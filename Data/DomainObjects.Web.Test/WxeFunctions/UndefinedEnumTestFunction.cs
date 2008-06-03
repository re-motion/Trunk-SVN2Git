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
using Remotion.Data.DomainObjects.Web.Test.Domain;
using Remotion.Mixins;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test.WxeFunctions
{
  using WxeTransactedFunction = WxeScopedTransactedFunction<ClientTransaction, ClientTransactionScope, ClientTransactionScopeManager>;

  [Serializable]
  public class UndefinedEnumTestFunction : WxeTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public UndefinedEnumTestFunction ()
    {
      ReturnUrl = "default.aspx";
    }

    // methods and properties

    public SearchObjectWithUndefinedEnum SearchObjectWithUndefinedEnum
    {
      get { return (SearchObjectWithUndefinedEnum) Variables["SearchObjectWithUndefinedEnum"]; }
      set { Variables["SearchObjectWithUndefinedEnum"] = value; }
    }

    public ClassWithUndefinedEnum ExistingObjectWithUndefinedEnum
    {
      get { return (ClassWithUndefinedEnum) Variables["ExistingObjectWithUndefinedEnum"]; }
      set { Variables["ExistingObjectWithUndefinedEnum"] = value; }
    }

    public ClassWithUndefinedEnum NewObjectWithUndefinedEnum
    {
      get { return (ClassWithUndefinedEnum) Variables["NewObjectWithUndefinedEnum"]; }
      set { Variables["NewObjectWithUndefinedEnum"] = value; }
    }

    private void Step1 ()
    {
      ExistingObjectWithUndefinedEnum = ClassWithUndefinedEnum.GetObject (DomainObjectIDs.ObjectWithUndefinedEnum);
      NewObjectWithUndefinedEnum = ClassWithUndefinedEnum.NewObject();
      SearchObjectWithUndefinedEnum = ObjectFactory.Create<SearchObjectWithUndefinedEnum>().With();
    }

    private WxePageStep Step2 = new WxePageStep ("UndefinedEnumTest.aspx");
  }
}
