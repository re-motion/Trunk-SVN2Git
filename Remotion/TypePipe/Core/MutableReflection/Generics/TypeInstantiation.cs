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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.Generics
{
  /// <summary>
  /// Represents a constructed generic type, i.e., a generic type definition that was instantiated with type arguments.
  /// This class is needed because the the original reflection classes do not work in combination with <see cref="CustomType"/> instances.
  /// </summary>
  /// <remarks>Instances of this class are returned by <see cref="TypeExtensions.MakeTypePipeGenericType"/>.</remarks>
  public class TypeInstantiation : CustomType
  {
    private const BindingFlags c_allMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    private readonly ReadOnlyCollection<Type> _interfaces;
    private readonly ReadOnlyCollection<FieldInfo> _fields;
    private readonly ReadOnlyCollection<ConstructorInfo> _constructors;
    private readonly ReadOnlyCollection<MethodInfo> _methods;
    private readonly ReadOnlyCollection<PropertyInfo> _properties;
    private readonly ReadOnlyCollection<EventInfo> _events;

    public TypeInstantiation (
        IMemberSelector memberSelector,
        IUnderlyingTypeFactory underlyingTypeFactory,
        ITypeInstantiator typeInstantiator,
        Type genericTypeDefinition)
        : base (
            memberSelector,
            underlyingTypeFactory,
            null,
            typeInstantiator.SubstituteGenericParameters (genericTypeDefinition.BaseType),
            genericTypeDefinition.Name, // Simple names of constructed type and generic type definition are equal.
            genericTypeDefinition.Namespace,
            typeInstantiator.GetFullName (genericTypeDefinition),
            genericTypeDefinition.Attributes,
            isGenericType: true,
            isGenericTypeDefinition: false,
            typeArguments: typeInstantiator.TypeArguments)
    {
      Assertion.IsTrue (genericTypeDefinition.IsGenericTypeDefinition);

      _interfaces = genericTypeDefinition.GetInterfaces().Select (typeInstantiator.SubstituteGenericParameters).ToList().AsReadOnly();
      _fields = genericTypeDefinition.GetFields (c_allMembers).Select (f => typeInstantiator.SubstituteGenericParameters (this, f)).ToList().AsReadOnly();
      _constructors = genericTypeDefinition.GetConstructors (c_allMembers).Select (c => typeInstantiator.SubstituteGenericParameters (this, c)).ToList().AsReadOnly();
      _methods = genericTypeDefinition.GetMethods (c_allMembers).Select (m => typeInstantiator.SubstituteGenericParameters (this, m)).ToList().AsReadOnly();
      _properties = genericTypeDefinition.GetProperties (c_allMembers).Select (p => typeInstantiator.SubstituteGenericParameters (this, p)).ToList().AsReadOnly();
      _events = genericTypeDefinition.GetEvents (c_allMembers).Select (e => typeInstantiator.SubstituteGenericParameters (this, e)).ToList().AsReadOnly();
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      throw new NotImplementedException();
    }

    public override InterfaceMapping GetInterfaceMap (Type interfaceType)
    {
      throw new NotImplementedException();
    }

    protected override IEnumerable<Type> GetAllInterfaces ()
    {
      return _interfaces;
    }

    protected override IEnumerable<FieldInfo> GetAllFields ()
    {
      return _fields;
    }

    protected override IEnumerable<ConstructorInfo> GetAllConstructors ()
    {
      return _constructors;
    }

    protected override IEnumerable<MethodInfo> GetAllMethods ()
    {
      return _methods;
    }

    protected override IEnumerable<PropertyInfo> GetAllProperties ()
    {
      return _properties;
    }

    protected override IEnumerable<EventInfo> GetAllEvents ()
    {
      return _events;
    }
  }
}