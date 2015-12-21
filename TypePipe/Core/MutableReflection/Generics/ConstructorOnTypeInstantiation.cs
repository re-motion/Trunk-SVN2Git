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
using System.Linq;
using System.Reflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.Generics
{
  /// <summary>
  /// Represents a constructor on a constructed type.
  /// </summary>
  public class ConstructorOnTypeInstantiation : CustomConstructorInfo
  {
    private readonly ConstructorInfo _constructor;
    private readonly IReadOnlyCollection<ParameterInfo> _parameters;

    public ConstructorOnTypeInstantiation (TypeInstantiation declaringType, ConstructorInfo constructor)
        : base (declaringType, ArgumentUtility.CheckNotNull ("constructor", constructor).Attributes)
    {
      _constructor = constructor;
      _parameters = constructor
          .GetParameters()
          .Select (p => new MemberParameterOnInstantiation (this, p)).Cast<ParameterInfo>().ToList().AsReadOnly();
    }

    public ConstructorInfo ConstructorOnGenericType
    {
      get { return _constructor; }
    }

    public override IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      return TypePipeCustomAttributeData.GetCustomAttributes (_constructor);
    }

    public override ParameterInfo[] GetParameters ()
    {
      return _parameters.ToArray();
    }
  }
}