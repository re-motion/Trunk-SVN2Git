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

namespace Remotion.Reflection.CodeGeneration.DynamicMethods
{
  /// <summary>
  /// Builds the IL code needed to access a property getter.
  /// </summary>
  public class PropertyGetterGenerator
  {
    private readonly ILGenerator _ilGenerator;

    public PropertyGetterGenerator (ILGenerator ilGenerator)
    {
      ArgumentUtility.CheckNotNull ("ilGenerator", ilGenerator);

      _ilGenerator = ilGenerator;
    }

    public void BuildWrapperMethod (MethodInfo wrappedGetMethod, Type wrapperReturnType, Type[] wrapperParameterTypes)
    {
      ArgumentUtility.CheckNotNull ("wrappedGetMethod", wrappedGetMethod);
      ArgumentUtility.CheckNotNull ("wrapperReturnType", wrapperReturnType);
      ArgumentUtility.CheckNotNullOrItemsNull ("wrapperParameterTypes", wrapperParameterTypes);
      if (!wrapperReturnType.IsAssignableFrom (wrappedGetMethod.ReturnType))
      {
        throw new ArgumentTypeException (
            "The ReturnType of the wrappedGetMethod cannot be assigned to the wrapperReturnType.",
            "wrappedGetMethod",
            wrapperReturnType,
            wrappedGetMethod.ReturnType);
      }

      _ilGenerator.Emit (OpCodes.Ldarg_0);
      _ilGenerator.Emit (OpCodes.Castclass, wrappedGetMethod.DeclaringType);
      _ilGenerator.EmitCall (OpCodes.Callvirt, wrappedGetMethod, null);
      //if (wrappedGetMethod.ReturnType.IsValueType)
      //  _ilGenerator.Emit (OpCodes.Box, wrappedGetMethod.ReturnType);

      _ilGenerator.Emit (OpCodes.Ret);
    }
  }
}