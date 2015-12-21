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
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.Development.UnitTesting.ObjectMothers.MutableReflection
{
  public static class GenericParameterDeclarationObjectMother
  {
    public static GenericParameterDeclaration Create (
        string name = "parameter",
        GenericParameterAttributes attributes = GenericParameterAttributes.None,
        Func<GenericParameterContext, IEnumerable<Type>> constraintProvider = null)
    {
      return new GenericParameterDeclaration (name, attributes, constraintProvider);
    }

    public static GenericParameterDeclaration[] CreateMultiple (int count)
    {
      return Enumerable.Range (1, count).Select (i => Create (name: "p" + i)).ToArray();
    }
  }
}