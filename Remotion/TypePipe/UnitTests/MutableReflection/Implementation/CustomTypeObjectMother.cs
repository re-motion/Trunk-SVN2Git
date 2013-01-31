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
using System.Collections.Generic;
using System.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  public static class CustomTypeObjectMother
  {
    public static CustomType Create (
        IMemberSelector memberSelector = null,
        IUnderlyingTypeFactory underlyingTypeFactory = null,
        Type declaringType = null,
        Type baseType = null,
        string name = "CustomType",
        string @namespace = "My",
        string fullName = "My.CustomType",
        TypeAttributes attributes = (TypeAttributes) 7,
        IEnumerable<Type> interfaces = null,
        IEnumerable<FieldInfo> fields = null,
        IEnumerable<ConstructorInfo> constructors = null,
        IEnumerable<MethodInfo> methods = null)
    {
      memberSelector = memberSelector ?? MockRepository.GenerateStub<IMemberSelector>();
      underlyingTypeFactory = underlyingTypeFactory ?? MockRepository.GenerateStub<IUnderlyingTypeFactory>();
      // Declaring type stays null.
      baseType = baseType ?? typeof (UnspecifiedType);

      var customType = new TestableCustomType (memberSelector, underlyingTypeFactory, declaringType, baseType, name, @namespace, fullName, attributes);
      customType.Interfaces = interfaces ?? Type.EmptyTypes;
      customType.Fields = fields ?? new FieldInfo[0];
      customType.Constructors = constructors ?? new ConstructorInfo[0];
      customType.Methods = methods ?? new MethodInfo[0];

      return customType;
    }

    public class UnspecifiedType { }
  }
}