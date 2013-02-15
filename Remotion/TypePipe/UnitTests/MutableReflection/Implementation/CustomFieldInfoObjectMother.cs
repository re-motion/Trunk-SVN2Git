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
using System.Reflection;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Implementation
{
  public static class CustomFieldInfoObjectMother
  {
    public static CustomFieldInfo Create (
        CustomType declaringType = null,
        string name = "CustomField",
        Type type = null,
        FieldAttributes attributes = (FieldAttributes) 7,
        IEnumerable<ICustomAttributeData> customAttributes = null)
    {
      declaringType = declaringType ?? CustomTypeObjectMother.Create();
      type = type ?? ReflectionObjectMother.GetSomeType();
      customAttributes = customAttributes ?? new ICustomAttributeData[0];

      return new TestableCustomFieldInfo (declaringType, name, type, attributes) { CustomAttributeDatas = customAttributes };
    }
  }
}