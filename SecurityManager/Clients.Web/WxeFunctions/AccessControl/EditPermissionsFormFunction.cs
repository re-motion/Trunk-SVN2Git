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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.ExecutionEngine;

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

    protected EditPermissionsFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditPermissionsFormFunction (ObjectID securableClassDefinitionObjectID)
      : base (securableClassDefinitionObjectID)
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
