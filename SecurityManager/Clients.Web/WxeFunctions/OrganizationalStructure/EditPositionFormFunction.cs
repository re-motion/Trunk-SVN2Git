// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
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
