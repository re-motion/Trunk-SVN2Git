// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
