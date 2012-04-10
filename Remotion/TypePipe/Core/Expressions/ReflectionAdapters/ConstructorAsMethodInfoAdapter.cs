// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Utilities;

namespace Remotion.TypePipe.Expressions.ReflectionAdapters
{
  /// <summary>
  /// Represents a constructor as a <see cref="MethodInfo"/>. This is required for calling a constructor using a 
  /// <see cref="MethodCallExpression"/>.
  /// </summary>
  public class ConstructorAsMethodInfoAdapter : MethodInfo
  {
    private readonly ConstructorInfo _constructorInfo;

    public ConstructorAsMethodInfoAdapter (ConstructorInfo constructorInfo)
    {
      ArgumentUtility.CheckNotNull ("constructorInfo", constructorInfo);
      _constructorInfo = constructorInfo;
    }

    public ConstructorInfo ConstructorInfo
    {
      get { return _constructorInfo; }
    }

    public override Type DeclaringType
    {
      get { return _constructorInfo.DeclaringType; }
    }

    public override MethodAttributes Attributes
    {
      get { return _constructorInfo.Attributes; }
    }

    public override Type ReturnType
    {
      get { return typeof (void); }
    }

    public override CallingConventions CallingConvention
    {
      get { return _constructorInfo.CallingConvention; }
    }

    public override ParameterInfo[] GetParameters ()
    {
      return _constructorInfo.GetParameters();
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      return _constructorInfo.GetCustomAttributes (inherit);
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      return _constructorInfo.IsDefined (attributeType, inherit);
    }

    public override MethodImplAttributes GetMethodImplementationFlags ()
    {
      return _constructorInfo.GetMethodImplementationFlags();
    }

    public override object Invoke (object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
      return _constructorInfo.Invoke (obj, invokeAttr, binder, parameters, culture);
    }

    public override MethodInfo GetBaseDefinition ()
    {
      return this;
    }

    public override ICustomAttributeProvider ReturnTypeCustomAttributes
    {
      get { throw new NotSupportedException(); }
    }

    public override string Name
    {
      get { return _constructorInfo.Name; }
    }

    public override Type ReflectedType
    {
      get { return _constructorInfo.ReflectedType; }
    }

    public override RuntimeMethodHandle MethodHandle
    {
      get { return _constructorInfo.MethodHandle; }
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      return _constructorInfo.GetCustomAttributes (attributeType, inherit);
    }

    public override string ToString ()
    {
      return _constructorInfo.ToString();
    }
  }
}