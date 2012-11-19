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
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  public static class MutableFieldInfoObjectMother
  {
    public static MutableFieldInfo Create (
        MutableType declaringType = null, string name = "_newField", Type type = null, FieldAttributes attributes = FieldAttributes.FieldAccessMask)
    {
      return CreateForNew (declaringType: declaringType, name: name, field: type, attributes: attributes);
    }

    public static MutableFieldInfo CreateForNew (
        MutableType declaringType = null, string name = "_newField", Type field = null, FieldAttributes attributes = FieldAttributes.FieldAccessMask)
    {
      return new MutableFieldInfo (
          declaringType ?? MutableTypeObjectMother.Create(),
          FieldDescriptorObjectMother.Create (name, field, attributes));
    }

    public static MutableFieldInfo CreateForExisting (MutableType declaringType = null, FieldInfo underlyingField = null)
    {
      return new MutableFieldInfo (
          declaringType ?? MutableTypeObjectMother.Create(),
          FieldDescriptorObjectMother.CreateForExisting (underlyingField));
    }
  }
}