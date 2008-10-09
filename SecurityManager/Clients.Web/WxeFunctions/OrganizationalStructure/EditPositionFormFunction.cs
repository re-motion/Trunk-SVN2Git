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
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditPositionFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditPositionFormFunction ()
    {
    }

    protected EditPositionFormFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }

    public EditPositionFormFunction (ITransactionMode transactionMode, ObjectID organizationalStructureObjectID)
      : base (transactionMode, organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Position Position
    {
      get { return (Position) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Position = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreatePosition ();
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditPositionForm), "UI/OrganizationalStructure/EditPositionForm.aspx");
  }
}
