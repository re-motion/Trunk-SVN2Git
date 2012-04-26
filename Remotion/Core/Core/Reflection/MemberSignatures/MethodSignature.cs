﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Remotion.Reflection.MemberSignatures.SignatureStringBuilding;
using Remotion.Utilities;

namespace Remotion.Reflection.MemberSignatures
{
  /// <summary>
  /// Represents a method signature and allows signatures to be compared to each other.
  /// </summary>
  public class MethodSignature
  {
    private static readonly MemberSignatureStringBuilderHelper s_helper = new MemberSignatureStringBuilderHelper();

    public static MethodSignature Create (MethodBase methodBase)
    {
      ArgumentUtility.CheckNotNull ("methodBase", methodBase);

      if (methodBase.IsGenericMethod && !methodBase.IsGenericMethodDefinition)
        throw new ArgumentException ("Closed generic methods are not supported.", "methodBase");

      var returnType = GetReturnType (methodBase);
      var parameterTypes = methodBase.GetParameters ().Select (p => p.ParameterType);
      var genericParameterCount = methodBase.IsGenericMethod ? methodBase.GetGenericArguments ().Length : 0;
      return new MethodSignature (returnType, parameterTypes, genericParameterCount);
    }

    private static Type GetReturnType (MethodBase methodBase)
    {
      var methodInfo = methodBase as MethodInfo;
      if (methodInfo == null)
      {
        Assertion.IsTrue (methodBase is ConstructorInfo);
        return typeof (void);
      }

      return methodInfo.ReturnType;
    }

    private readonly Type _returnType;
    private readonly IEnumerable<Type> _parameterTypes;
    private readonly int _genericParameterCount;

    public MethodSignature (Type returnType, IEnumerable<Type> parameterTypes, int genericParameterCount)
    {
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);

      _returnType = returnType;
      _parameterTypes = parameterTypes;
      _genericParameterCount = genericParameterCount;
    }

    public Type ReturnType
    {
      get { return _returnType; }
    }

    public IEnumerable<Type> ParameterTypes
    {
      get { return _parameterTypes; }
    }

    public int GenericParameterCount
    {
      get { return _genericParameterCount; }
    }

    public override string ToString ()
    {
      var sb = new StringBuilder ();

      s_helper.AppendTypeString (sb, ReturnType);

      sb.Append ("(");
      s_helper.AppendSeparatedTypeStrings (sb, ParameterTypes);
      sb.Append (")");

      if (GenericParameterCount > 0)
        sb.Append ("`").Append (GenericParameterCount);

      return sb.ToString ();
    }
  }
}