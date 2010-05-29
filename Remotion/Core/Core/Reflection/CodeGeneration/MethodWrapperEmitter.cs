// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  /// <summary>
  /// Builds the IL code needed to wrap a method call.
  /// </summary>
  public class MethodWrapperEmitter
  {
    public MethodWrapperEmitter ()
    {

    }

    public void EmitStaticMethodBody (ILGenerator ilGenerator, MethodInfo wrappedMethod, Type wrapperReturnType, Type[] wrapperParameterTypes)
    {
      ArgumentUtility.CheckNotNull ("ilGenerator", ilGenerator);
      ArgumentUtility.CheckNotNull ("wrappedMethod", wrappedMethod);
      ArgumentUtility.CheckNotNull ("wrapperReturnType", wrapperReturnType);
      ArgumentUtility.CheckNotNullOrItemsNull ("wrapperParameterTypes", wrapperParameterTypes);
      if (!wrapperReturnType.IsAssignableFrom (wrappedMethod.ReturnType))
      {
        throw new ArgumentTypeException (
            "The ReturnType of the wrappedMethod cannot be assigned to the wrapperReturnType.",
            "wrappedMethod",
            wrapperReturnType,
            wrappedMethod.ReturnType);
      }

      EmitInstanceArgument(ilGenerator, wrappedMethod);
      EmitMethodArguments (ilGenerator, wrappedMethod);

      EmitMethodCall (ilGenerator, wrappedMethod);

      EmitReturnStatement (ilGenerator, wrappedMethod, wrapperReturnType);

    }

    private void EmitInstanceArgument (ILGenerator ilGenerator, MethodInfo wrappedMethod)
    {
      if (wrappedMethod.IsStatic)
        return;

      ilGenerator.Emit (OpCodes.Ldarg_0);
      ilGenerator.Emit (OpCodes.Castclass, wrappedMethod.DeclaringType);
    }

    private void EmitMethodArguments (ILGenerator ilGenerator, MethodInfo wrappedMethod)
    {
      if (wrappedMethod.GetParameters().Length > 0)
      {
        ilGenerator.Emit (OpCodes.Ldarg_1);

        var parameterInfo = wrappedMethod.GetParameters()[0];
        if (parameterInfo.ParameterType.IsValueType)
          ilGenerator.Emit (OpCodes.Unbox_Any, parameterInfo.ParameterType);
        else
          ilGenerator.Emit (OpCodes.Castclass, parameterInfo.ParameterType);
      }
    }

    private void EmitMethodCall (ILGenerator ilGenerator, MethodInfo wrappedMethod)
    {
      if (wrappedMethod.IsStatic)
        ilGenerator.Emit (OpCodes.Call, wrappedMethod);
      else
        ilGenerator.Emit (OpCodes.Callvirt, wrappedMethod);
    }

    private void EmitReturnStatement (ILGenerator ilGenerator, MethodInfo wrappedGetMethod, Type wrapperReturnType)
    {
      if (wrapperReturnType == typeof (void))
      {
        //NOP
      }
      else if (wrappedGetMethod.ReturnType.IsValueType)
      {
        ilGenerator.Emit (OpCodes.Box, wrappedGetMethod.ReturnType);
      }

      ilGenerator.Emit (OpCodes.Ret);
    }
  }
}