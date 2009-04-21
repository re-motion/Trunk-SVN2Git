// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
