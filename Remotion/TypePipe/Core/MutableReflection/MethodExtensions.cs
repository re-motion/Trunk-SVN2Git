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
using System.Reflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  public static class MethodExtensions
  {
    public static MethodInfo MakeTypePipeGenericMethod (this MethodInfo genericMethodDefinition, params Type[] typeArguments)
    {
      ArgumentUtility.CheckNotNull ("genericMethodDefinition", genericMethodDefinition);
      ArgumentUtility.CheckNotNullOrEmpty ("typeArguments", typeArguments);

      if (!genericMethodDefinition.IsGenericMethodDefinition)
      {
        var message = string.Format (
            "'{0}' is not a generic method definition. {1} may only be called on a method for which MethodInfo.IsGenericMethodDefinition is true.",
            genericMethodDefinition.Name,
            MethodInfo.GetCurrentMethod ().Name);
        throw new InvalidOperationException (message);
      }

      var typeParameters = genericMethodDefinition.GetGenericArguments();
      GenericArgumentUtility.ValidateGenericArguments (typeParameters, typeArguments, genericMethodDefinition.Name);

      var instantiationInfo = new MethodInstantiationInfo (genericMethodDefinition, typeArguments);
      return instantiationInfo.Instantiate();
    }
  }
}