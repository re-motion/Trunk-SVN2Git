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
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Remotion.Security;
using Remotion.Security.Metadata;
using Remotion.Security.Configuration;
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Security.UI
{
  public class WebSecurityAdapter : IWebSecurityAdapter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WebSecurityAdapter ()
    {
    }

    // methods and properties

    public bool HasAccess (ISecurableObject securableObject, Delegate handler)
    {
      if (handler == null)
        return true;

      if (SecurityFreeSection.IsActive)
        return true;

      List<DemandTargetPermissionAttribute> attributes = GetPermissionAttributes (handler.GetInvocationList ());

      bool hasAccess = true;
      foreach (DemandTargetPermissionAttribute attribute in attributes)
      {
        switch (attribute.PermissionSource)
        {
          case PermissionSource.WxeFunction:
            hasAccess &= WxeFunction.HasAccess (attribute.FunctionType);
            break;
          case PermissionSource.SecurableObject:
            SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
            if (securableObject == null)
              hasAccess &= securityClient.HasStatelessMethodAccess (attribute.SecurableClass, attribute.MethodName);
            else
              hasAccess &= securityClient.HasMethodAccess (securableObject, attribute.MethodName);
            break;
          default:
            throw new InvalidOperationException (string.Format (
                "Value '{0}' is not supported by the PermissionSource property of the DemandTargetPermissionAttribute.",
                attribute.PermissionSource));
        }

        if (!hasAccess)
          break;
      }

      return hasAccess;
    }

    //public void CheckAccess (ISecurableObject securableObject, Delegate handler)
    //{
    //  throw new Exception ("The method or operation is not implemented.");
    //}

    private List<DemandTargetPermissionAttribute> GetPermissionAttributes (Delegate[] delegates)
    {
      List<DemandTargetPermissionAttribute> attributes = new List<DemandTargetPermissionAttribute> ();
      foreach (Delegate handler in delegates)
      {
        DemandTargetPermissionAttribute attribute = (DemandTargetPermissionAttribute) Attribute.GetCustomAttribute (
            handler.Method,
            typeof (DemandTargetPermissionAttribute),
            false);

        if (attribute != null)
          attributes.Add (attribute);
      }

      return attributes;
    }
  }
}
