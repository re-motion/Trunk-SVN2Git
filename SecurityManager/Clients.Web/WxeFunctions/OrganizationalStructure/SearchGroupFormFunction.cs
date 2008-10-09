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
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class SearchGroupFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SearchGroupFormFunction ()
    {
    }

    protected SearchGroupFormFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }

    public SearchGroupFormFunction (ITransactionMode transactionMode)
      : base (transactionMode)
    {
    }

    // methods and properties

    public Group SelectedGroup
    {
      get { return (Group) Variables["Group"]; }
      set { Variables["Group"] = value; }
    }

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (SearchGroupForm), "UI/OrganizationalStructure/SearchGroupForm.aspx");
  }
}
