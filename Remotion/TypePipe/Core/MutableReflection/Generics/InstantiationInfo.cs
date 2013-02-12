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
using Remotion.Collections;
using Remotion.ServiceLocation;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.Generics
{
  /// <summary>
  /// A class that holds the information needed to construct a generic type.
  /// </summary>
  /// <remarks>This is used by <see cref="TypeInstantiation"/> as the key in a context dictionary to break cyclic dependencies.</remarks>
  public class InstantiationInfo
  {
    private readonly IUnderlyingTypeFactory _underlyingTypeFactory = SafeServiceLocator.Current.GetInstance<IUnderlyingTypeFactory>();

    private readonly Type _genericTypeDefinition;
    private readonly ReadOnlyCollection<Type> _typeArguments;
    private readonly object[] _key;

    public InstantiationInfo (Type genericTypeDefinition, IEnumerable<Type> typeArguments)
    {
      ArgumentUtility.CheckNotNull ("genericTypeDefinition", genericTypeDefinition);
      ArgumentUtility.CheckNotNull ("typeArguments", typeArguments);
      Assertion.IsTrue (genericTypeDefinition.IsGenericTypeDefinition);

      _genericTypeDefinition = genericTypeDefinition;
      _typeArguments = typeArguments.ToList().AsReadOnly();
      Assertion.IsTrue (genericTypeDefinition.GetGenericArguments().Length == _typeArguments.Count);
      _key = new object[] { genericTypeDefinition }.Concat (_typeArguments.Cast<object>()).ToArray();
    }

    public Type GenericTypeDefinition
    {
      get { return _genericTypeDefinition; }
    }

    public ReadOnlyCollection<Type> TypeArguments
    {
      get { return _typeArguments; }
    }

    public Type MakeGenericType (Dictionary<InstantiationInfo, TypeInstantiation> instantiations)
    {
      var typeInstantiation = instantiations.GetValueOrDefault (this);
      if (typeInstantiation != null)
        return typeInstantiation;

      // Make RuntimeType if all type arguments are RuntimeTypes.
      if (_typeArguments.All (typeArg => typeArg.IsRuntimeType()))
        return _genericTypeDefinition.MakeGenericType (TypeArguments.ToArray());

      var memberSelector = new MemberSelector (new BindingFlagsEvaluator());
      return new TypeInstantiation (memberSelector, _underlyingTypeFactory, this, instantiations);
    }

    public override bool Equals (object obj)
    {
      var other = obj as InstantiationInfo;
      if (other == null)
        return false;

      return _key.SequenceEqual (other._key);
    }

    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_key);
    }
  }
}