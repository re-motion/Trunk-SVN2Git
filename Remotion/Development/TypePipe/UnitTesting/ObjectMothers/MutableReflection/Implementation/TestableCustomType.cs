// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.Development.TypePipe.UnitTesting.ObjectMothers.MutableReflection.Implementation
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