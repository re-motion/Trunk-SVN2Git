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
using System.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  public static class CustomPropertyInfoObjectMother
  {
    public static CustomPropertyInfo Create (
        CustomType declaringType = null,
        string name = "UnspecifiedProperty",
        PropertyAttributes attributes = (PropertyAttributes) 7,
        IEnumerable<ICustomAttributeData> customAttributes = null,
        CustomMethodInfo getMethod = null,
        CustomMethodInfo setMethod = null,
        ParameterInfo[] indexParameters = null)
    {
      declaringType = declaringType ?? CustomTypeObjectMother.Create();
      customAttributes = customAttributes ?? new ICustomAttributeData[0];
      // Getter stays null.
      // Setters stays null, but if both are null then create a getter.
      if (getMethod == null && setMethod == null)
        getMethod = CustomMethodInfoObjectMother.Create (returnParameter: CustomParameterInfoObjectMother.Create (position: -1, type: typeof (int)));
      indexParameters = indexParameters ?? new ParameterInfo[0];

      return new TestableCustomPropertyInfo (declaringType, name, attributes, getMethod, setMethod)
             {
                 CustomAttributeDatas = customAttributes,
                 IndexParameters = indexParameters
             };
    }
  }
}