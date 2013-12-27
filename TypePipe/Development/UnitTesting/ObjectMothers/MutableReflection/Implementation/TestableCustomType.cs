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
using System.Collections.Generic;
using System.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.TypePipe.Development.UnitTesting.ObjectMothers.MutableReflection.Implementation
{
  public class TestableCustomType : CustomType
  {
    public TestableCustomType (
        string name,
        string @namespace,
        TypeAttributes attributes,
        Type genericTypeDefinition,
        IEnumerable<Type> typeArguments)
        : base (name, @namespace, attributes, genericTypeDefinition, typeArguments)
    {
    }

    public IEnumerable<ICustomAttributeData> CustomAttributeDatas;
    public IEnumerable<Type> NestedTypes; 
    public IEnumerable<Type> Interfaces;
    public IEnumerable<FieldInfo> Fields;
    public IEnumerable<ConstructorInfo> Constructors;
    public IEnumerable<MethodInfo> Methods;
    public IEnumerable<PropertyInfo> Properties;
    public IEnumerable<EventInfo> Events;

    public void CallSetBaseType (Type baseType)
    {
      SetBaseType (baseType);
    }

    public void CallSetDeclaringType (Type declaringType)
    {
      SetDeclaringType (declaringType);
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      return CustomAttributeDatas;
    }

    public override IEnumerable<Type> GetAllNestedTypes ()
    {
      return NestedTypes;
    }

    public override IEnumerable<Type> GetAllInterfaces ()
    {
      return Interfaces;
    }

    public override IEnumerable<FieldInfo> GetAllFields ()
    {
      return Fields;
    }

    public override IEnumerable<ConstructorInfo> GetAllConstructors ()
    {
      return Constructors;
    }

    public override IEnumerable<MethodInfo> GetAllMethods ()
    {
      return Methods;
    }

    public override IEnumerable<PropertyInfo> GetAllProperties ()
    {
      return Properties;
    }

    public override IEnumerable<EventInfo> GetAllEvents ()
    {
      return Events;
    }
  }
}