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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Holds all values required to declare a method, constructor, or property parameter.
  /// </summary>
  /// <remarks>
  /// This is used by <see cref="ProxyType"/> when declaring methods, constructors, or indexed properties.
  /// </remarks>
  public class ParameterDeclaration
  {
    public static readonly ParameterDeclaration[] EmptyParameters = new ParameterDeclaration[0];

    public static ParameterDeclaration CreateEquivalent (ParameterInfo parameterInfo)
    {
      ArgumentUtility.CheckNotNull ("parameterInfo", parameterInfo);

      return new ParameterDeclaration (parameterInfo.ParameterType, parameterInfo.Name, parameterInfo.Attributes);
    }

    public static ReadOnlyCollection<ParameterDeclaration> CreateForEquivalentSignature (MethodBase methodBase)
    {
      ArgumentUtility.CheckNotNull ("methodBase", methodBase);

      return methodBase.GetParameters().Select (CreateEquivalent).ToList().AsReadOnly();
    }

    private readonly Type _type;
    private readonly string _name;
    private readonly ParameterAttributes _attributes;
    private readonly ParameterExpression _expression;

    public ParameterDeclaration (Type type, string name, ParameterAttributes attributes = ParameterAttributes.None)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      // Name may be null.

      if (type == typeof (void))
        throw new ArgumentException ("Parameter cannot be of type void.", "type");

      _type = type;
      _name = name;
      _attributes = attributes;
      _expression = Microsoft.Scripting.Ast.Expression.Parameter (type, name);
    }

    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return _name; }
    }

    public ParameterAttributes Attributes
    {
      get { return _attributes; }
    }

    public ParameterExpression Expression
    {
      get { return _expression; }
    }
  }
}