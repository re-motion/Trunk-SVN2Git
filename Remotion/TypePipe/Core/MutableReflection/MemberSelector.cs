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
using System.Linq;
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Selects members based on <see cref="BindingFlags"/> and other criteria. This is used to implement member access operations in 
  /// <see cref="MutableType"/>.
  /// </summary>
  public class MemberSelector : IMemberSelector
  {
    private readonly IBindingFlagsEvaluator _bindingFlagsEvaluator;

    public MemberSelector (IBindingFlagsEvaluator bindingFlagsEvaluator)
    {
      ArgumentUtility.CheckNotNull ("bindingFlagsEvaluator", bindingFlagsEvaluator);

      _bindingFlagsEvaluator = bindingFlagsEvaluator;
    }

    public IEnumerable<FieldInfo> SelectFields (IEnumerable<FieldInfo> candidates, BindingFlags bindingAttr)
    {
      ArgumentUtility.CheckNotNull ("candidates", candidates);

      return candidates.Where (field => _bindingFlagsEvaluator.HasRightAttributes (field.Attributes, bindingAttr));
    }

    public IEnumerable<T> SelectMethods<T> (IEnumerable<T> candidates, BindingFlags bindingAttr) where T : MethodBase
    {
      ArgumentUtility.CheckNotNull ("candidates", candidates);

      return candidates.Where (ctor => _bindingFlagsEvaluator.HasRightAttributes (ctor.Attributes, bindingAttr));
    }

    public T SelectSingleMethod<T> (
        Binder binder,
        BindingFlags bindingAttr,
        IEnumerable<T> candidates,
        Type[] typesOrNull,
        ParameterModifier[] modifiersOrNull)
      where T : MethodBase
    {
      ArgumentUtility.CheckNotNull ("binder", binder);
      ArgumentUtility.CheckNotNull ("candidates", candidates);

      if (typesOrNull == null && modifiersOrNull != null)
        throw new ArgumentException ("Modifiers must not be specified if types are null.", "modifiersOrNull");

      if (typesOrNull == null)
      {
        var candidatesArray = SelectMethods (candidates, bindingAttr).ToArray ();
        
        if (candidatesArray.Length == 0)
          return null;

        if (candidatesArray.Length > 1)
          throw new AmbiguousMatchException();

        return candidatesArray.Single();
      }

      return (T) binder.SelectMethod (bindingAttr, candidates.ToArray(), typesOrNull, modifiersOrNull);
    }

    public FieldInfo SelectSingleField (IEnumerable<FieldInfo> candidates, BindingFlags bindingAttr)
    {
      ArgumentUtility.CheckNotNull ("candidates", candidates);

      var fieldCollection = SelectFields (candidates, bindingAttr).ConvertToCollection();

      if (fieldCollection.Count == 0)
        return null;
      
      if (fieldCollection.Count > 1)
        throw new AmbiguousMatchException();

      return fieldCollection.Single ();
    }
  }
}