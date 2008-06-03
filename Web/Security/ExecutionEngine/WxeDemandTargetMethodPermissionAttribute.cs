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
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.Web.Security.ExecutionEngine
{
  public class WxeDemandTargetMethodPermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandTargetMethodPermissionAttribute (object methodNameEnum)
      : base (MethodType.Instance)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodNameEnum", methodNameEnum);
      Type enumType = enumValue.GetType ();

      CheckDeclaringTypeOfMethodNameEnum (enumValue);

      MethodName = enumValue.ToString ();
      SecurableClass = enumType.DeclaringType;
    }

    public WxeDemandTargetMethodPermissionAttribute (object methodNameEnum, Type securableClass)
      : base (MethodType.Instance)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodNameEnum", methodNameEnum);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      CheckDeclaringTypeOfMethodNameEnum (enumValue, securableClass);

      MethodName = enumValue.ToString ();
      SecurableClass = securableClass;
    }

    public WxeDemandTargetMethodPermissionAttribute (string methodName)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      MethodName = methodName;
    }

    public WxeDemandTargetMethodPermissionAttribute (string methodName, Type securableClass)
      : base (MethodType.Instance)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      MethodName = methodName;
      SecurableClass = securableClass;
    }

    // methods and properties

    public new string ParameterName
    {
      get { return base.ParameterName; }
      set { base.ParameterName = value; }
    }
  }
}
