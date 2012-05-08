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
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Reflection.MemberSignatures;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.UnitTests.Expressions;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class UnderlyingMethodInfoDescriptorTest
  {
    private IRelatedMethodFinder _relatedMethodFinderMock;

    [SetUp]
    public void SetUp ()
    {
      _relatedMethodFinderMock = MockRepository.GenerateStrictMock<IRelatedMethodFinder>();
    }

    [Test]
    public void Create_ForNew ()
    {
      var name = "Method";
      var attributes = MethodAttributes.Abstract;
      var returnType = ReflectionObjectMother.GetSomeType();
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var baseMethod = ReflectionObjectMother.GetSomeMethod();
      var isGenericMethod = BooleanObjectMother.GetRandomBoolean();
      var isGenericMethodDefinition = BooleanObjectMother.GetRandomBoolean();
      var containsGenericParameters = BooleanObjectMother.GetRandomBoolean();
      var body = ExpressionTreeObjectMother.GetSomeExpression (returnType);


      var descriptor = UnderlyingMethodInfoDescriptor.Create (
          name,
          attributes,
          returnType,
          parameterDeclarations.AsOneTime(),
          baseMethod,
          isGenericMethod,
          isGenericMethodDefinition,
          containsGenericParameters,
          body);

      Assert.That (descriptor.UnderlyingSystemMethodBase, Is.Null);
      Assert.That (descriptor.Name, Is.EqualTo (name));
      Assert.That (descriptor.Attributes, Is.EqualTo (attributes));
      Assert.That (descriptor.ReturnType, Is.SameAs (returnType));
      Assert.That (descriptor.ParameterDeclarations, Is.EqualTo (parameterDeclarations));
      Assert.That (descriptor.BaseMethod, Is.SameAs (baseMethod));
      Assert.That (descriptor.IsGenericMethod, Is.EqualTo (isGenericMethod));
      Assert.That (descriptor.IsGenericMethodDefinition, Is.EqualTo (isGenericMethodDefinition));
      Assert.That (descriptor.ContainsGenericParameters, Is.EqualTo (containsGenericParameters));
      Assert.That (descriptor.Body, Is.SameAs (body));
    }

    [Test]
    public void Create_ForNew_NullBaseMethod ()
    {
      var body = ExpressionTreeObjectMother.GetSomeExpression (typeof (int));
      MethodInfo baseMethod = null;
      var descriptor = UnderlyingMethodInfoDescriptor.Create (
          "Method", MethodAttributes.Abstract, typeof (int), ParameterDeclaration.EmptyParameters, baseMethod, false, false, false, body);

      Assert.That (descriptor.BaseMethod, Is.Null);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException), ExpectedMessage =
        "The body's return type must be assignable to the method return type.\r\nParameter name: body")]
    public void Create_ForNew_ThrowsForInvalidBodyReturnType ()
    {
      var body = ExpressionTreeObjectMother.GetSomeExpression (typeof (string));
      UnderlyingMethodInfoDescriptor.Create (
          "Method", MethodAttributes.Abstract, typeof (int), ParameterDeclaration.EmptyParameters, null, false, false, false, body);
    }
    
    [Test]
    public void Create_ForExisting ()
    {
      int v;
      var originalMethod = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method ("string", out v, 1.0, null));
      var descriptor = UnderlyingMethodInfoDescriptor.Create (originalMethod, _relatedMethodFinderMock);
      Assert.That (descriptor.UnderlyingSystemMethodBase, Is.SameAs (originalMethod));
      Assert.That (descriptor.Name, Is.EqualTo (originalMethod.Name));
      Assert.That (descriptor.Attributes, Is.EqualTo (originalMethod.Attributes));
      Assert.That (descriptor.ReturnType, Is.SameAs (originalMethod.ReturnType));

      var expectedParamterDecls =
          new[]
          {
              new { Type = typeof (string), Name = "s", Attributes = ParameterAttributes.None },
              new { Type = typeof (int).MakeByRefType(), Name = "i", Attributes = ParameterAttributes.Out },
              new { Type = typeof (double), Name = "d", Attributes = ParameterAttributes.In },
              new { Type = typeof (object), Name = "o", Attributes = ParameterAttributes.In | ParameterAttributes.Out },
          };
      var actualParameterDecls = descriptor.ParameterDeclarations.Select (pd => new { pd.Type, pd.Name, pd.Attributes });
      Assert.That (actualParameterDecls, Is.EqualTo (expectedParamterDecls));
      Assert.That (descriptor.BaseMethod, Is.Null);

      Assert.That (descriptor.Body, Is.TypeOf<OriginalBodyExpression> ());
      var originalBodyExpression = (OriginalBodyExpression) descriptor.Body;
      Assert.That (originalBodyExpression.Type, Is.SameAs (originalMethod.ReturnType));
      Assert.That (originalBodyExpression.Arguments, Is.EqualTo (descriptor.ParameterDeclarations.Select (pd => pd.Expression)));
    }

    [Test]
    public void Create_ForExisting_ChangesVisibilityProtectedOrInternalToProtected ()
    {
      var originalMethod = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.ProtectedInternalMethod());
      Assert.That (originalMethod.IsFamilyOrAssembly, Is.True);

      var descriptor = UnderlyingMethodInfoDescriptor.Create (originalMethod, _relatedMethodFinderMock);

      var visibility = descriptor.Attributes & MethodAttributes.MemberAccessMask;
      Assert.That (visibility, Is.EqualTo (MethodAttributes.Family));
    }

    [Test]
    public void Create_ForExisting_OverridingMethod ()
    {
      var method = typeof (DomainType).GetMethod ("ToString");
      var baseMethodStub = MockRepository.GenerateStub<MethodInfo>();
      var fakeRootDefinition = ReflectionObjectMother.GetSomeMethod();
      _relatedMethodFinderMock
          .Expect (mock => mock.GetBaseMethod (method.Name, MethodSignature.Create (method), method.DeclaringType.BaseType))
          .Return (baseMethodStub);
      baseMethodStub.Stub (stub => stub.GetBaseDefinition()).Return (fakeRootDefinition);

      var descriptor = UnderlyingMethodInfoDescriptor.Create (method, _relatedMethodFinderMock);

      _relatedMethodFinderMock.VerifyAllExpectations();
      Assert.That (descriptor.BaseMethod, Is.SameAs (baseMethodStub));
      Assert.That (descriptor.BaseMethod, Is.Not.EqualTo (fakeRootDefinition));
    }

    private class DomainType
    {
      public int Method (string s, out int i, [In] double d, [In, Out] object o)
      {
        Dev.Null = s;
        i = 0;
        Dev.Null = d;
        Dev.Null = o;
        return 0;
      }

      protected internal string ProtectedInternalMethod () { return null; }

      // Some method that has a different root definition
      public override string ToString () { return null; }
    }
  }
}