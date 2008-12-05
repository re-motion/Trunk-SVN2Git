// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
