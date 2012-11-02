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
using System.Linq;
using System.Reflection;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  public static class UnderlyingParameterInfoDescriptorObjectMother
  {
    public static readonly UnderlyingParameterInfoDescriptor[] Empty = new UnderlyingParameterInfoDescriptor[0];

    public static UnderlyingParameterInfoDescriptor CreateForNew (
        Type parameterType = null, string name = "parameter", ParameterAttributes attributes = ParameterAttributes.In)
    {
      var parameterDeclartion = new ParameterDeclaration (parameterType ?? typeof (UnspecifiedType), name, attributes);
      return UnderlyingParameterInfoDescriptor.CreateFromDeclarations (new[] { parameterDeclartion }).Single();
    }

    public static UnderlyingParameterInfoDescriptor CreateForExisting (ParameterInfo parameter = null)
    {
      string s;
      parameter = parameter
                  ?? NormalizingMemberInfoFromExpressionUtility.GetMethod ((UnspecifiedType obj) => obj.Method (out s)).GetParameters().Single();

      return UnderlyingParameterInfoDescriptor.CreateFromMethodBase ((MethodBase) parameter.Member).Single (p => p.UnderlyingSystemInfo == parameter);
    }

    public static UnderlyingParameterInfoDescriptor[] CreateMultiple (int count)
    {
      return UnderlyingParameterInfoDescriptor.CreateFromDeclarations (ParameterDeclarationObjectMother.CreateMultiple (count)).ToArray();
    }

    private class UnspecifiedType
    {
      public void Method (out string parameterName)
      {
        parameterName = "";
      }
    }
  }
}