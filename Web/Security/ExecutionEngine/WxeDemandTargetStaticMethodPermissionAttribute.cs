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
  //[DemandTargetStaticMethodPermission (Akt.Methods.Protokollieren)] // default: containing type of nested enum -> Akt
  //[DemandTargetStaticMethodPermission (Akt.Methods.Protokollieren, typeof (Sachakt))]

  public class WxeDemandTargetStaticMethodPermissionAttribute : WxeDemandTargetPermissionAttribute
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public WxeDemandTargetStaticMethodPermissionAttribute (object methodNameEnum)
      : base (MethodType.Static)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodNameEnum", methodNameEnum);
      Type enumType = enumValue.GetType();

      CheckDeclaringTypeOfMethodNameEnum (enumValue);

      Initialize (enumValue.ToString (), enumType.DeclaringType);
    }

    public WxeDemandTargetStaticMethodPermissionAttribute (object methodNameEnum, Type securableClass)
      : base (MethodType.Static)
    {
      Enum enumValue = ArgumentUtility.CheckNotNullAndType<Enum> ("methodNameEnum", methodNameEnum);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      CheckDeclaringTypeOfMethodNameEnum (enumValue, securableClass);

      Initialize (enumValue.ToString (), securableClass);
    }

    public WxeDemandTargetStaticMethodPermissionAttribute (string methodName, Type securableClass)
      : base (MethodType.Static)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("securableClass", securableClass, typeof (ISecurableObject));

      Initialize (methodName, securableClass);
    }

    // methods and properties

    private void Initialize (string methodName, Type securableClass)
    {
      SecurableClass = securableClass;
      MethodName = methodName;
    }
  }
}
