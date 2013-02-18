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

using System.Collections.Generic;
using System.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using System.Linq;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  public static class CustomMethodInfoObjectMother
  {
    public static CustomMethodInfo Create (
        CustomType declaringType = null,
        string name = "CustomMethod",
        MethodAttributes attributes = (MethodAttributes) 7,
        ParameterInfo returnParameter = null,
        IEnumerable<ParameterInfo> parameters = null,
        MethodInfo baseDefinition = null,
        IEnumerable<ICustomAttributeData> customAttributes = null)
    {
      declaringType = declaringType ?? CustomTypeObjectMother.Create();
      returnParameter = returnParameter ?? CustomParameterInfoObjectMother.Create (position: -1, type: typeof (void));
      parameters = parameters ?? new ParameterInfo[0];
      // Base definition stays null.
      customAttributes = customAttributes ?? new ICustomAttributeData[0];

      return new TestableCustomMethodInfo (declaringType, name, attributes)
             {
                 ReturnParameter_ = returnParameter,
                 Parameters = parameters.ToArray(),
                 BaseDefinition = baseDefinition,
                 CustomAttributeDatas = customAttributes.ToArray()
             };
    }
  }
}