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
using Remotion.SecurityManager.Clients.Web.UI.AccessControl;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  [Serializable]
  public class SecurableClassDefinitionListFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SecurableClassDefinitionListFormFunction ()
    {
    }

    // TODO: Make protected once delegation works
    public SecurableClassDefinitionListFormFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }

    public SecurableClassDefinitionListFormFunction (ITransactionMode transactionMode, ObjectID tenantID)
      : base (transactionMode, tenantID)
    {
    }

    // methods and properties

    private void Step1 ()
    {
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (SecurableClassDefinitionListForm), "UI/AccessControl/SecurableClassDefinitionListForm.aspx");

  }
}
