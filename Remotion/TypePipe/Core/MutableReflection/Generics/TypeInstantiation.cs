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
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Text;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.Generics
{
  /// <summary>
  /// Represents a constructed generic <see cref="Type"/>, i.e., a generic type definition that was instantiated with type arguments.
  /// This class is needed because the the original reflection classes do not work in combination with <see cref="CustomType"/> instances.
  /// </summary>
  /// <remarks>Instances of this class are returned by <see cref="TypeExtensions.MakeTypePipeGenericType"/> and implement value equality.</remarks>
  public class TypeInstantiation : CustomType
  {
    private const BindingFlags c_allMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    private static string GetFullName (TypeInstantiationInfo info)
    {
      var typeArgumentString = SeparatedStringBuilder.Build (",", info.TypeArguments, t => "[" + t.AssemblyQualifiedName + "]");
      return string.Format ("{0}[{1}]", info.GenericTypeDefinition.FullName, typeArgumentString);
    }

    private readonly TypeInstantiationInfo _instantiationInfo;
    private readonly TypeInstantiationContext _instantiationContext;
    private readonly IDictionary<Type, Type> _parametersToArguments;

    private readonly ReadOnlyCollection<Type> _interfaces;
    private readonly ReadOnlyCollection<FieldInfo> _fields;
    private readonly ReadOnlyCollection<ConstructorInfo> _constructors;
    private readonly ReadOnlyCollection<MethodInfo> _methods;
    private readonly ReadOnlyCollection<PropertyInfo> _properties;
    private readonly ReadOnlyCollection<EventInfo> _events;

    public TypeInstantiation (IMemberSelector memberSelector, TypeInstantiationInfo instantiationInfo, TypeInstantiationContext instantiationContext)
        : base (
            memberSelector,
            ArgumentUtility.CheckNotNull ("instantiationInfo", instantiationInfo).GenericTypeDefinition.Name,
            instantiationInfo.GenericTypeDefinition.Namespace,
            GetFullName (instantiationInfo),
            instantiationInfo.GenericTypeDefinition.Attributes,
            genericTypeDefinition: instantiationInfo.GenericTypeDefinition,
            typeArguments: instantiationInfo.TypeArguments)
    {
      ArgumentUtility.CheckNotNull ("instantiationContext", instantiationContext);

      _instantiationInfo = instantiationInfo;
      _instantiationContext = instantiationContext;

      // Even though the _genericTypeDefinition includes the type parameters of the enclosing type(s) (if any), declaringType.GetGenericArguments() 
      // will return objects not equal to this type's generic parameters. Since the call to SetDeclaringType below needs to replace the those type 
      // parameters with type arguments, add a mapping for the declaring type's generic parameters in addition to this type's generic parameters.

      var genericTypeDefinition = instantiationInfo.GenericTypeDefinition;
      var declaringType = genericTypeDefinition.DeclaringType;
      // ReSharper disable ConditionIsAlwaysTrueOrFalse // ReSharper is wrong here, declaringType can be null.
      var outerMapping = declaringType != null ? declaringType.GetGenericArguments().Zip (instantiationInfo.TypeArguments) : new Tuple<Type, Type>[0];
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      var mapping = genericTypeDefinition.GetGenericArguments().Zip (instantiationInfo.TypeArguments);
      _parametersToArguments = outerMapping.Concat (mapping).ToDictionary (t => t.Item1, t => t.Item2);

      // Add own instantation to context before substituting any generic parameters. 
      instantiationContext.Add (instantiationInfo, this);

      // ReSharper disable ConditionIsAlwaysTrueOrFalse // ReSharper is wrong here, declaringType can be null.
      if (declaringType != null)
        SetDeclaringType (SubstituteGenericParameters (declaringType));
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      if (genericTypeDefinition.BaseType != null)
        SetBaseType (SubstituteGenericParameters (genericTypeDefinition.BaseType));

      var interfaces = genericTypeDefinition.GetInterfaces().Select (SubstituteGenericParameters);
      var fields = genericTypeDefinition.GetFields (c_allMembers).Select (f => new FieldOnTypeInstantiation (this, f));
      var constructors = genericTypeDefinition.GetConstructors (c_allMembers).Select (c => new ConstructorOnTypeInstantiation (this, c));
      var methods = genericTypeDefinition.GetMethods (c_allMembers).Select (m => new MethodOnTypeInstantiation (this, m)).ToList();
      var methodMapping = methods.ToDictionary (m => m.MethodOnGenericType);
      var properties = genericTypeDefinition.GetProperties (c_allMembers).Select (p => CreateProperty (p, methodMapping));
      var events = genericTypeDefinition.GetEvents (c_allMembers).Select (e => CreateEvent (e, methodMapping));

      _interfaces = interfaces.ToList().AsReadOnly();
      _fields = fields.Cast<FieldInfo>().ToList().AsReadOnly();
      _constructors = constructors.Cast<ConstructorInfo>().ToList().AsReadOnly();
      _methods = methods.Cast<MethodInfo>().ToList().AsReadOnly();
      _properties = properties.Cast<PropertyInfo>().ToList().AsReadOnly();
      _events = events.Cast<EventInfo>().ToList().AsReadOnly();
    }

    public Type SubstituteGenericParameters (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _instantiationContext.SubstituteGenericParameters (type, _parametersToArguments);
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as Type);
    }

    public override bool Equals (Type type)
    {
      var other = type as TypeInstantiation;
      if (other == null)
        return false;

      return _instantiationInfo.Equals (other._instantiationInfo);
    }

    public override int GetHashCode ()
    {
      return _instantiationInfo.GetHashCode();
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      return TypePipeCustomAttributeData.GetCustomAttributes (GetGenericTypeDefinition());
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

    private PropertyOnTypeInstantiation CreateProperty (PropertyInfo genericProperty, Dictionary<MethodInfo, MethodOnTypeInstantiation> methodMapping)
    {
      var getMethod = GetMethodOrNull (methodMapping, genericProperty.GetGetMethod (true));
      var setMethod = GetMethodOrNull (methodMapping, genericProperty.GetSetMethod (true));

      return new PropertyOnTypeInstantiation (this, genericProperty, getMethod, setMethod);
    }

    private EventOnTypeInstantiation CreateEvent (EventInfo genericEvent, Dictionary<MethodInfo, MethodOnTypeInstantiation> methodMapping)
    {
      var addMethod = GetMethodOrNull (methodMapping, genericEvent.GetAddMethod (true));
      var removeMethod = GetMethodOrNull (methodMapping, genericEvent.GetRemoveMethod (true));
      var raiseMethod = GetMethodOrNull (methodMapping, genericEvent.GetRaiseMethod (true));

      return new EventOnTypeInstantiation (this, genericEvent, addMethod, removeMethod, raiseMethod);
    }

    private static MethodOnTypeInstantiation GetMethodOrNull (Dictionary<MethodInfo, MethodOnTypeInstantiation> methodMapping, MethodInfo genericMethod)
    {
      return genericMethod != null ? methodMapping[genericMethod] : null;
    }
  }
}