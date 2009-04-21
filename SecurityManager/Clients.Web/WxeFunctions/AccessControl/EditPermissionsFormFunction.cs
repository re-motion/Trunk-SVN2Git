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
using Remotion.SecurityManager.Clients.Web.UI.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  [Serializable]
  public class EditPermissionsFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditPermissionsFormFunction ()
    {
    }

    protected EditPermissionsFormFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }

    public EditPermissionsFormFunction (ITransactionMode transactionMode, ObjectID securableClassDefinitionObjectID)
      : base (transactionMode, securableClassDefinitionObjectID)
    {
    }

    // methods and properties
    public SecurableClassDefinition SecurableClassDefinition
    {
      get { return (SecurableClassDefinition) CurrentObject; }
      set { CurrentObject = value; }
    }

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (EditPermissionsForm), "UI/AccessControl/EditPermissionsForm.aspx");
  }
}
