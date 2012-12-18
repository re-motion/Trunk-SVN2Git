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
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using System.Linq;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Descriptors;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Descriptors
{
  [TestFixture]
  public class ParameterDescriptorTest
  {
    [Test]
    public void EmptyParameters ()
    {
      Assert.That (ParameterDescriptor.EmptyParameters, Is.Empty);
    }

    [Test]
    public void Create_ForNew ()
    {
      var type = ReflectionObjectMother.GetSomeType();
      var position = 7;
      var name = "parameterName";
      var attributes = ParameterAttributes.Optional | ParameterAttributes.In;
      var declaration = new ParameterDeclaration (type, name, attributes);

      var descriptor = ParameterDescriptor.Create (declaration, position);

      CheckDescriptor (descriptor, null, type, name, position, attributes, type, false, Type.EmptyTypes);
    }

    [Test]
    public void Create_ForNew_VoidType ()
    {
      var declaration = ParameterDeclaration.CreateReturnParameter (typeof (void));

      var descriptor = ParameterDescriptor.Create (declaration, 7);

      Assert.That (descriptor.Expression, Is.Null);
    }

    [Test]
    public void Create_ForExisting ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method (0, out Dev<string>.Dummy));
      var underlyingParameter = method.GetParameters()[1];

      var descriptor = ParameterDescriptor.Create (underlyingParameter);

      var type = typeof (string);
      CheckDescriptor (
          descriptor,
          underlyingParameter,
          type.MakeByRefType(),
          "parameterName",
          1,
          ParameterAttributes.Out,
          type,
          expectedIsByRef: true,
          expectedCustomAttributeTypes: new[] { typeof (AbcAttribute), typeof (DefAttribute) });
    }

    [Test]
    public void Expression_ReturnsSameInstance ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method (0, out Dev<string>.Dummy));
      var underlyingParameter = method.GetParameters ()[1];

      var descriptor = ParameterDescriptor.Create (underlyingParameter);

      var result1 = descriptor.Expression;
      var result2 = descriptor.Expression;

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void CreateFromDeclarations ()
    {
      var declaration = ParameterDeclarationObjectMother.Create();

      var descriptor = ParameterDescriptor.CreateFromDeclarations (new[] { declaration }.AsOneTime()).Single();

      var type = declaration.Type;
      Assert.That (type.IsByRef, Is.False);
      CheckDescriptor (descriptor, null, type, declaration.Name, 0, declaration.Attributes, type, false, Type.EmptyTypes);
    }

    [Test]
    public void CreateFromMethodBase ()
    {
      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod (() => Method (0, out Dev<string>.Dummy));

      var descriptor = ParameterDescriptor.CreateFromMethodBase (method).Last();

      var underlyingParameter = method.GetParameters().Last();
      var type = typeof (string);
      CheckDescriptor (
          descriptor,
          underlyingParameter,
          type.MakeByRefType(),
          "parameterName",
          1,
          ParameterAttributes.Out,
          type,
          expectedIsByRef: true,
          expectedCustomAttributeTypes: new[] { typeof (AbcAttribute), typeof (DefAttribute) });
    }

    private static void CheckDescriptor (ParameterDescriptor descriptor, ParameterInfo expectedParameterInfo, Type expectedType, string expectedName, int expectedPosition, ParameterAttributes expectedAttributes, Type expectedExpressionType, bool expectedIsByRef, Type[] expectedCustomAttributeTypes)
    {
      Assert.That (descriptor.UnderlyingSystemInfo, Is.SameAs (expectedParameterInfo));
      Assert.That (descriptor.Type, Is.SameAs (expectedType));
      Assert.That (descriptor.Position, Is.EqualTo (expectedPosition));
      Assert.That (descriptor.Name, Is.EqualTo (expectedName));
      Assert.That (descriptor.Attributes, Is.EqualTo (expectedAttributes));
      Assert.That (descriptor.Expression.Name, Is.EqualTo (expectedName));
      Assert.That (descriptor.Expression.Type, Is.SameAs (expectedExpressionType));
      Assert.That (descriptor.Expression.IsByRef, Is.EqualTo (expectedIsByRef));

      // OutAttribute is added automatically for out parameters.
      var reducedCustomAttributeTypes =
          descriptor.CustomAttributeDataProvider.Invoke()
              .Select (a => a.Type)
              .Where (t => t != typeof (OutAttribute));
      Assert.That (reducedCustomAttributeTypes, Is.EquivalentTo (expectedCustomAttributeTypes));
    }

    private void Method (int i, [Abc, Def] out string parameterName) { Dev.Null = i; parameterName = ""; }

    private class AbcAttribute : Attribute { }
    private class DefAttribute : Attribute { }
  }
}