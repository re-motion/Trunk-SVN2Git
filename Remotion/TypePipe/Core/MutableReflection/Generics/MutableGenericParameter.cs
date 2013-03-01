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
using Remotion.FunctionalProgramming;

namespace Remotion.TypePipe.MutableReflection.Generics
{
  /// <summary>
  /// Represents a generic type parameter on a generic type or method definition.
  /// </summary>
  public class MutableGenericParameter : CustomType, IMutableMember
  {
    private const BindingFlags c_allMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private readonly CustomAttributeContainer _customAttributeContainer = new CustomAttributeContainer();

    private readonly int _position;
    private readonly GenericParameterAttributes _genericParameterAttributes;

    private MemberInfo _declaringMember;
    private ReadOnlyCollection<Type> _interfaceConstraints = EmptyTypes.ToList().AsReadOnly();

    public MutableGenericParameter (
        IMemberSelector memberSelector,
        int position,
        string name,
        string @namespace,
        GenericParameterAttributes genericParameterAttributes)
        : base (
            memberSelector,
            name,
            @namespace,
            fullName: null,
            attributes: TypeAttributes.Public,
            isGenericType: false,
            genericTypeDefinition: null,
            typeArguments: EmptyTypes)
    {
      ArgumentUtility.CheckNotNull ("memberSelector", memberSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      // Namespace may be null.
      Assertion.IsTrue (position >= 0);

      _position = position;
      _genericParameterAttributes = genericParameterAttributes;

      SetBaseType (typeof (object));
    }

    public ProxyType MutableDeclaringType
    {
      get { return (ProxyType) DeclaringType; }
    }

    public ReadOnlyCollection<CustomAttributeDeclaration> AddedCustomAttributes
    {
      get { return _customAttributeContainer.AddedCustomAttributes; }
    }

    public override bool IsGenericParameter
    {
      get { return true; }
    }

    public override int GenericParameterPosition
    {
      get { return _position; }
    }

    public override GenericParameterAttributes GenericParameterAttributes
    {
      get { return _genericParameterAttributes; }
    }

    public override MethodBase DeclaringMethod
    {
      get { return _declaringMember as MethodBase; }
    }

    public void InitializeDeclaringMember (MemberInfo declaringMember)
    {
      ArgumentUtility.CheckNotNull ("declaringMember", declaringMember);
      Assertion.IsTrue (declaringMember is ProxyType || declaringMember is MutableMethodInfo);

      if (_declaringMember != null)
        throw new InvalidOperationException ("InitializeDeclaringMember must be called exactly once.");

      SetDeclaringType (declaringMember as Type ?? declaringMember.DeclaringType);
      _declaringMember = declaringMember;
    }

    public void SetBaseTypeConstraint (Type baseTypeConstraint)
    {
      ArgumentUtility.CheckNotNull ("baseTypeConstraint", baseTypeConstraint);

      if (!baseTypeConstraint.IsClass)
        throw new ArgumentException ("A base type constraint must be a class.", "baseTypeConstraint");

      SetBaseType (baseTypeConstraint);
    }

    public void SetInterfaceConstraints (IEnumerable<Type> interfaceConstraints)
    {
      ArgumentUtility.CheckNotNull ("interfaceConstraints", interfaceConstraints);
      var ifcConstraints = interfaceConstraints.ConvertToCollection();

      if (!ifcConstraints.All (c => c.IsInterface))
        throw new ArgumentException ("All interface constraints must be interfaces.", "interfaceConstraints");

      _interfaceConstraints = ifcConstraints.ToList().AsReadOnly();
    }

    public override Type[] GetGenericParameterConstraints ()
    {
      return new[] { BaseType }.Where (c => c != typeof (object)).Concat (_interfaceConstraints).ToArray();
    }

    public void AddCustomAttribute (CustomAttributeDeclaration customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);

      _customAttributeContainer.AddCustomAttribute (customAttribute);
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      return _customAttributeContainer.AddedCustomAttributes.Cast<ICustomAttributeData>();
    }

    public override InterfaceMapping GetInterfaceMap (Type interfaceType)
    {
      throw new NotImplementedException();
    }

    protected override IEnumerable<Type> GetAllInterfaces ()
    {
      Assertion.IsNotNull (BaseType);

      return _interfaceConstraints.Concat (BaseType.GetInterfaces()).Distinct();
    }

    protected override IEnumerable<FieldInfo> GetAllFields ()
    {
      Assertion.IsNotNull (BaseType);

      return BaseType.GetFields (c_allMembers);
    }

    protected override IEnumerable<ConstructorInfo> GetAllConstructors ()
    {
      if (_genericParameterAttributes.IsSet (GenericParameterAttributes.DefaultConstructorConstraint))
        yield return new GenericParameterDefaultConstructor (this);
    }

    protected override IEnumerable<MethodInfo> GetAllMethods ()
    {
      Assertion.IsNotNull (BaseType);

      return BaseType.GetMethods (c_allMembers);
    }

    protected override IEnumerable<PropertyInfo> GetAllProperties ()
    {
      Assertion.IsNotNull (BaseType);

      return BaseType.GetProperties (c_allMembers);
    }

    protected override IEnumerable<EventInfo> GetAllEvents ()
    {
      Assertion.IsNotNull (BaseType);

      return BaseType.GetEvents (c_allMembers);
    }
  }
}