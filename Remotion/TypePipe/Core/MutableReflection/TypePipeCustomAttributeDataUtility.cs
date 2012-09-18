﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Creates <see cref="TypePipeCustomAttributeData"/> from <see cref="CustomAttributeDeclaration"/> and <see cref="CustomAttributeData"/> instances.
  /// </summary>
  public static class TypePipeCustomAttributeDataUtility
  {
    public static TypePipeCustomAttributeData Create (CustomAttributeDeclaration customAttributeDeclaration)
    {
      ArgumentUtility.CheckNotNull ("customAttributeDeclaration", customAttributeDeclaration);

      var ctorArguments = customAttributeDeclaration.ConstructorArguments.Select (
          v => new TypePipeCustomAttributeTypedArgument (GetArgumentType (v), v));

      var namedArguments = customAttributeDeclaration.NamedArguments.Select (
          na => new TypePipeCustomAttributeNamedArgument (
                    na.MemberInfo,
                    new TypePipeCustomAttributeTypedArgument (
                        GetArgumentType (na.Value),
                        na.Value)));

      return new TypePipeCustomAttributeData (customAttributeDeclaration.Constructor, ctorArguments, namedArguments);
    }

    public static TypePipeCustomAttributeData Create (CustomAttributeData customAttributeData)
    {
      ArgumentUtility.CheckNotNull ("customAttributeData", customAttributeData);

      return new TypePipeCustomAttributeData (
          customAttributeData.Constructor,
          customAttributeData.ConstructorArguments.Select (ca => new TypePipeCustomAttributeTypedArgument (ca.ArgumentType, ca.Value)),
          customAttributeData.NamedArguments.Select (
              na =>
              new TypePipeCustomAttributeNamedArgument (
                  na.MemberInfo,
                  new TypePipeCustomAttributeTypedArgument (na.TypedValue.ArgumentType, na.TypedValue.Value))));
    }

    private static Type GetArgumentType (object value)
    {
      return value != null ? value.GetType() : typeof (string);
    }
  }
}