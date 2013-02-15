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
  /// Represents a constructed generic type, i.e., a generic type definition that was instantiated with type arguments.
  /// This class is needed because the the original reflection classes do not work in combination with <see cref="CustomType"/> instances.
  /// </summary>
  /// <remarks>Instances of this class are returned by <see cref="TypeExtensions.MakeTypePipeGenericType"/>.</remarks>
  public class TypeInstantiation : CustomType
  {
    private const BindingFlags c_allMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    private static string GetFullName (InstantiationInfo info)
    {
      var typeArgumentString = SeparatedStringBuilder.Build (",", info.TypeArguments, t => "[" + t.AssemblyQualifiedName + "]");
      return string.Format ("{0}[{1}]", info.GenericTypeDefinition.FullName, typeArgumentString);
    }

    private readonly Type _genericTypeDefinition;
    private readonly Dictionary<InstantiationInfo, TypeInstantiation> _instantiations;
    private readonly Dictionary<Type, Type> _parametersToArguments;

    private readonly ReadOnlyCollection<Type> _interfaces;
    private readonly ReadOnlyCollection<FieldInfo> _fields;
    private readonly ReadOnlyCollection<ConstructorInfo> _constructors;
    private readonly ReadOnlyCollection<MethodInfo> _methods;
    private readonly ReadOnlyCollection<PropertyInfo> _properties;
    private readonly ReadOnlyCollection<EventInfo> _events;

    public TypeInstantiation (
        IMemberSelector memberSelector,
        InstantiationInfo instantiationInfo,
        Dictionary<InstantiationInfo, TypeInstantiation> instantiations)
        : base (
            memberSelector,
            ArgumentUtility.CheckNotNull ("instantiationInfo", instantiationInfo).GenericTypeDefinition.Name,
            instantiationInfo.GenericTypeDefinition.Namespace,
            GetFullName (instantiationInfo),
            instantiationInfo.GenericTypeDefinition.Attributes,
            isGenericType: true,
            isGenericTypeDefinition: false,
            typeArguments: instantiationInfo.TypeArguments)
    {
      ArgumentUtility.CheckNotNull ("instantiations", instantiations);

      _genericTypeDefinition = instantiationInfo.GenericTypeDefinition;
      _instantiations = instantiations;

      // Even though the _genericTypeDefinition includes the type parameters of the enclosing type(s) (if any), declaringType.GetGenericArguments() 
      // will return objects not equal to this type's generic parameters. Since the call to SetDeclaringType below needs to replace the those type 
      // parameters with type arguments, add a mapping for the declaring type's generic parameters in addition to this type's generic parameters.

      var declaringType = _genericTypeDefinition.DeclaringType;
      // ReSharper disable ConditionIsAlwaysTrueOrFalse // ReSharper is wrong here, declaringType can be null.
      var outerMapping = declaringType != null ? declaringType.GetGenericArguments().Zip (instantiationInfo.TypeArguments) : new Tuple<Type, Type>[0];
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      var mapping = _genericTypeDefinition.GetGenericArguments().Zip (instantiationInfo.TypeArguments);
      _parametersToArguments = outerMapping.Concat (mapping).ToDictionary (t => t.Item1, t => t.Item2);

      // Add own instantation to context before substituting any generic parameters. 
      instantiations.Add (instantiationInfo, this);

      // ReSharper disable ConditionIsAlwaysTrueOrFalse // ReSharper is wrong here, declaringType can be null.
      if (declaringType != null)
        SetDeclaringType (SubstituteGenericParameters (declaringType));
      // ReSharper restore ConditionIsAlwaysTrueOrFalse
      if (_genericTypeDefinition.BaseType != null)
        SetBaseType (SubstituteGenericParameters (_genericTypeDefinition.BaseType));

      // TODO Review: Write one test using a mock for member selector, check that binding flags are correct.
      var interfaces = _genericTypeDefinition.GetInterfaces().Select (SubstituteGenericParameters);
      var fields = _genericTypeDefinition.GetFields (c_allMembers).Select (f => new FieldOnTypeInstantiation (this, f));
      var constructors = _genericTypeDefinition.GetConstructors (c_allMembers).Select (c => new ConstructorOnTypeInstantiation (this, c));
      var methods = _genericTypeDefinition.GetMethods (c_allMembers).Select (m => new MethodOnTypeInstantiation (this, m)).ToList();
      var methodMapping = methods.ToDictionary (m => m.MethodOnGenericType);
      var properties = _genericTypeDefinition.GetProperties (c_allMembers).Select (p => CreateProperty (p, methodMapping));
      var events = _genericTypeDefinition.GetEvents (c_allMembers).Select (e => CreateEvent (e, methodMapping));

      _interfaces = interfaces.ToList().AsReadOnly();
      _fields = fields.Cast<FieldInfo>().ToList().AsReadOnly();
      _constructors = constructors.Cast<ConstructorInfo>().ToList().AsReadOnly();
      _methods = methods.Cast<MethodInfo>().ToList().AsReadOnly();
      _properties = properties.Cast<PropertyInfo>().ToList().AsReadOnly();
      _events = events.Cast<EventInfo>().ToList().AsReadOnly();
    }

    public override Type UnderlyingSystemType
    {
      get { throw new NotSupportedException ("Property UnderlyingSystemType is not supported."); }
    }

    public override Type GetGenericTypeDefinition ()
    {
      return _genericTypeDefinition;
    }

    public Type SubstituteGenericParameters (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var typeArgument = _parametersToArguments.GetValueOrDefault (type);
      if (typeArgument != null)
        return typeArgument;

      if (!type.IsGenericType)
        return type;

      Assertion.IsFalse (type.IsArray, "Not yet supported, TODO 5409");

      var oldTypeArguments = type.GetGenericArguments();
      var newTypeArguments = oldTypeArguments.Select (SubstituteGenericParameters).ToList();

      // No substitution necessary (this is an optimization only).
      if (oldTypeArguments.SequenceEqual (newTypeArguments))
        return type;

      var genericTypeDefinition = type.GetGenericTypeDefinition();
      var instantiationInfo = new InstantiationInfo (genericTypeDefinition, newTypeArguments);

      return instantiationInfo.Instantiate (_instantiations);
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      // TODO Review: Return TypePipeCustomAttributeData.Get... (_genericTypeDefinition, false)
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

    private PropertyOnTypeInstantiation CreateProperty (PropertyInfo genericProperty, Dictionary<MethodInfo, MethodOnTypeInstantiation> methodMapping)
    {
      var getMethod = methodMapping.GetValueOrDefault (genericProperty.GetGetMethod (true));
      var setMethod = methodMapping.GetValueOrDefault (genericProperty.GetSetMethod (true));

      return new PropertyOnTypeInstantiation (this, genericProperty, getMethod, setMethod);
    }

    private object CreateEvent (EventInfo genericEvent, Dictionary<MethodInfo, MethodOnTypeInstantiation> methodMapping)
    {
      var addMethod = methodMapping.GetValueOrDefault (genericEvent.GetAddMethod (true));
      var removeMethod = methodMapping.GetValueOrDefault (genericEvent.GetRemoveMethod (true));
      var raiseMethod = methodMapping.GetValueOrDefault (genericEvent.GetRaiseMethod (true));

      return new EventOnTypeInstantiation (this, genericEvent, addMethod, removeMethod, raiseMethod);
    }
  }
}