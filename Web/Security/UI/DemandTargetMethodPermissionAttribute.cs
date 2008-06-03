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

namespace Remotion.Web.Security.UI
{
  public class DemandTargetMethodPermissionAttribute : DemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public DemandTargetMethodPermissionAttribute (object methodEnum)
      : base (methodEnum)
    {
    }

    public DemandTargetMethodPermissionAttribute (object methodEnum, Type securableClass)
      : base (methodEnum, securableClass)
    {
    }

    public DemandTargetMethodPermissionAttribute (string methodName)
      : base (methodName)
    {
    }

    public DemandTargetMethodPermissionAttribute (string methodName, Type securableClass)
      : base (methodName, securableClass)
    {
    }

    // methods and properties
  }
}
