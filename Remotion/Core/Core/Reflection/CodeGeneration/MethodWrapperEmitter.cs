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
using System.Linq;
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

    public void EmitStaticMethodBody (ILGenerator ilGenerator, MethodInfo wrappedMethod, Type[] wrapperParameterTypes, Type wrapperReturnType)
    {
      ArgumentUtility.CheckNotNull ("ilGenerator", ilGenerator);
      ArgumentUtility.CheckNotNull ("wrappedMethod", wrappedMethod);
      ArgumentUtility.CheckNotNullOrItemsNull ("wrapperParameterTypes", wrapperParameterTypes);
      ArgumentUtility.CheckNotNull ("wrapperReturnType", wrapperReturnType);
      if (wrappedMethod.ContainsGenericParameters)
        throw new ArgumentException ("Open generic method definitions are not supported by the MethodWrapperGenerator.", "wrappedMethod");
      CheckParameterCount (wrappedMethod, wrapperParameterTypes);
      CheckInstanceParameterType (wrappedMethod, wrapperParameterTypes);
      CheckParameterTypes (wrappedMethod, wrapperParameterTypes);
      CheckReturnTypes (wrappedMethod, wrapperReturnType);

      EmitInstanceArgument (ilGenerator, wrappedMethod, wrapperParameterTypes);
      EmitMethodArguments (ilGenerator, wrappedMethod, wrapperParameterTypes);
      EmitMethodCall (ilGenerator, wrappedMethod);
      EmitReturnStatement (ilGenerator, wrappedMethod, wrapperReturnType);
    }

    private void CheckParameterCount (MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      if (wrapperParameterTypes.Length != wrappedMethod.GetParameters().Length + 1)
      {
        throw new ArgumentException (
            string.Format (
                "The number of elements in the wrapperParameterTypes array ({0}) does not match the number of parameters required for invoking the wrappedMethod ({1}).",
                wrapperParameterTypes.Length,
                wrappedMethod.GetParameters().Length + 1),
            "wrapperParameterTypes");
      }
    }

    private void CheckInstanceParameterType (MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      if (!wrapperParameterTypes[0].IsAssignableFrom (wrappedMethod.DeclaringType))
      {
        throw new ArgumentTypeException (
            string.Format (
                "The wrapperParameterType #0 ('{0}') cannot be assigned to the declaring type ('{1}') of the wrappedMethod.",
                wrapperParameterTypes[0].Name,
                wrappedMethod.DeclaringType.Name),
            "wrapperParameterTypes",
            wrappedMethod.DeclaringType,
            wrapperParameterTypes[0]);
      }
    }

    private void CheckParameterTypes (MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      foreach (var wrappedParameter in wrappedMethod.GetParameters())
      {
        var wrapperParameterType = wrapperParameterTypes[wrappedParameter.Position + 1];
        if (!wrapperParameterType.IsAssignableFrom (wrappedParameter.ParameterType))
        {
          throw new ArgumentTypeException (
              string.Format (
                  "The wrapperParameterType #{1} ('{0}') cannot be assigned to the type ('{2}') of parameter '{3}' of the wrappedMethod.",
                  wrapperParameterType.Name,
                  wrappedParameter.Position + 1,
                  wrappedParameter.ParameterType.Name,
                  wrappedParameter.Name),
              "wrapperParameterTypes",
              wrappedParameter.ParameterType,
              wrapperParameterType);
        }

        if (wrappedParameter.IsOut)
        {
          throw new ArgumentException (
              string.Format (
                  "Parameter '{0}' of the wrappedMethod is an out parameter, but out parameters are not supported by the MethodWrapperGenerator.",
                  wrappedParameter.Name),
              "wrappedMethod");
        }
        
        if (wrappedParameter.ParameterType.IsByRef)
        {
          throw new ArgumentException (
              string.Format (
                  "Parameter '{0}' of the wrappedMethod is a by-ref parameter, but by-ref parameters are not supported by the MethodWrapperGenerator.",
                  wrappedParameter.Name),
              "wrappedMethod");
        }

        if (wrappedParameter.IsOptional)
        {
          throw new ArgumentException (
              string.Format (
                  "Parameter '{0}' of the wrappedMethod is an optional parameter, but optional parameters are not supported by the MethodWrapperGenerator.",
                  wrappedParameter.Name),
              "wrappedMethod");
        }
      }
    }

    private void CheckReturnTypes (MethodInfo wrappedMethod, Type wrapperReturnType)
    {
      if (!wrapperReturnType.IsAssignableFrom (wrappedMethod.ReturnType))
      {
        throw new ArgumentTypeException (
            string.Format (
                "The wrapperReturnType ('{0}') cannot be assigned from the return type ('{1}') of the wrappedMethod.",
                wrapperReturnType.Name,
                wrappedMethod.ReturnType.Name),
            "wrapperReturnType",
            wrappedMethod.ReturnType,
            wrapperReturnType);
      }
    }

    private void EmitInstanceArgument (ILGenerator ilGenerator, MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      if (wrappedMethod.IsStatic)
        return;

      ilGenerator.Emit (OpCodes.Ldarg_0);

      if (wrappedMethod.DeclaringType.IsValueType)
      {
        ilGenerator.DeclareLocal (wrappedMethod.DeclaringType);
        if (wrapperParameterTypes[0] != wrappedMethod.DeclaringType)
          ilGenerator.Emit (OpCodes.Unbox_Any, wrappedMethod.DeclaringType);
        ilGenerator.Emit (OpCodes.Stloc_0);
        ilGenerator.Emit (OpCodes.Ldloca_S, 0);
      }
      else
        ilGenerator.Emit (OpCodes.Castclass, wrappedMethod.DeclaringType);
    }

    private void EmitMethodArguments (ILGenerator ilGenerator, MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      foreach (var parameter in wrappedMethod.GetParameters())
      {
        ilGenerator.Emit (OpCodes.Ldarg_S, (byte) (parameter.Position + 1));

        if (wrapperParameterTypes[parameter.Position + 1] == parameter.ParameterType)
          ilGenerator.Emit (OpCodes.Nop);
        else if (parameter.ParameterType.IsValueType)
          ilGenerator.Emit (OpCodes.Unbox_Any, parameter.ParameterType);
        else
          ilGenerator.Emit (OpCodes.Castclass, parameter.ParameterType);
      }
    }

    private void EmitMethodCall (ILGenerator ilGenerator, MethodInfo wrappedMethod)
    {
      if (wrappedMethod.IsVirtual)
        ilGenerator.Emit (OpCodes.Callvirt, wrappedMethod);
      else
        ilGenerator.Emit (OpCodes.Call, wrappedMethod);
    }

    private void EmitReturnStatement (ILGenerator ilGenerator, MethodInfo wrappedMethod, Type wrapperReturnType)
    {
      if (wrapperReturnType == typeof (void))
        ilGenerator.Emit (OpCodes.Nop);
      else if (wrapperReturnType == wrappedMethod.ReturnType)
        ilGenerator.Emit (OpCodes.Nop);
      else if (wrappedMethod.ReturnType.IsValueType)
        ilGenerator.Emit (OpCodes.Box, wrappedMethod.ReturnType);

      ilGenerator.Emit (OpCodes.Ret);
    }
  }
}