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
using Microsoft.Scripting.Ast;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using System.Linq;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.UnitTests.Expressions;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableConstructorInfoTest
  {
    private MutableType _declaringType;
    
    private MutableConstructorInfo _mutableCtor;
    private UnderlyingConstructorInfoDescriptor _descriptor;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = MutableTypeObjectMother.Create();

      var parameters = UnderlyingParameterInfoDescriptorObjectMother.CreateMultiple (2);
      _descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew (parameterDescriptors: parameters);
      _mutableCtor = Create (_descriptor);
    }

    [Test]
    public void Initialization ()
    {
      var ctorInfo = new MutableConstructorInfo (_declaringType, _descriptor);

      Assert.That (ctorInfo.DeclaringType, Is.SameAs (_declaringType));
      Assert.That (((IMutableMember) _mutableCtor).DeclaringType, Is.SameAs (_declaringType));
      Assert.That (_mutableCtor.Body, Is.SameAs (_descriptor.Body));
    }

    [Test]
    public void UnderlyingSystemConstructorInfo ()
    {
      var descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForExisting ();
      Assert.That (descriptor.UnderlyingSystemMember, Is.Not.Null);

      var ctorInfo = Create (descriptor);

      Assert.That (ctorInfo.UnderlyingSystemConstructorInfo, Is.SameAs (descriptor.UnderlyingSystemMember));
    }

    [Test]
    public void UnderlyingSystemConstructorInfo_ForNull ()
    {
      var descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew ();
      Assert.That (descriptor.UnderlyingSystemMember, Is.Null);

      var ctorInfo = Create (descriptor);

      Assert.That (ctorInfo.UnderlyingSystemConstructorInfo, Is.SameAs (ctorInfo));
    }

    [Test]
    public void IsNewConstructor_True ()
    {
      var descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew ();
      Assert.That (descriptor.UnderlyingSystemMember, Is.Null);

      var ctorInfo = Create (descriptor);

      Assert.That (ctorInfo.IsNew, Is.True);
    }

    [Test]
    public void IsNewConstructor_False ()
    {
      var descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForExisting ();
      Assert.That (descriptor.UnderlyingSystemMember, Is.Not.Null);

      var ctorInfo = Create (descriptor);

      Assert.That (ctorInfo.IsNew, Is.False);
    }

    [Test]
    public void IsModified_False ()
    {
      var ctorInfo = MutableConstructorInfoObjectMother.Create();
      Assert.That (ctorInfo.IsModified, Is.False);
    }

    [Test]
    public void IsModified_True ()
    {
      var ctorInfo = MutableConstructorInfoObjectMother.Create ();

      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (void));
      ctorInfo.SetBody (ctx => fakeBody);

      Assert.That (ctorInfo.IsModified, Is.True);
    }

    [Test]
    public void Attributes ()
    {
      Assert.That (_mutableCtor.Attributes, Is.EqualTo (_descriptor.Attributes));
    }

    [Test]
    public void CallingConvention ()
    {
      Assert.That (_mutableCtor.CallingConvention, Is.EqualTo (CallingConventions.HasThis));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_mutableCtor.Name, Is.EqualTo (_descriptor.Name));
    }

    [Test]
    public void ParameterExpressions ()
    {
      var parameterDeclarations = UnderlyingParameterInfoDescriptorObjectMother.CreateMultiple (2);
      var ctorInfo = CreateWithParameters (parameterDeclarations);

      Assert.That (ctorInfo.ParameterExpressions, Is.EqualTo (parameterDeclarations.Select (pd => pd.Expression)));
    }

    [Test]
    public void CanSetBody ()
    {
      var newInaccessibleCtor = Create (UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew (attributes: MethodAttributes.Assembly));
      var newAccessibleCtor = Create (UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew (attributes: MethodAttributes.Family));

      var existingInaccesibleCtor = Create (UnderlyingConstructorInfoDescriptorObjectMother.CreateForExisting (
          NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType (7))));
      var existingAccessibleCtor = Create (UnderlyingConstructorInfoDescriptorObjectMother.CreateForExisting (
          NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType ())));

      Assert.That (newInaccessibleCtor.CanSetBody, Is.True);
      Assert.That (newAccessibleCtor.CanSetBody, Is.True);
      Assert.That (existingInaccesibleCtor.CanSetBody, Is.False);
      Assert.That (existingAccessibleCtor.CanSetBody, Is.True);
    }

    [Test]
    public void SetBody ()
    {
      var fakeBody = ExpressionTreeObjectMother.GetSomeExpression (typeof (object));
      Func<ConstructorBodyModificationContext, Expression> bodyProvider = context =>
      {
        Assert.That (_mutableCtor.ParameterExpressions, Is.Not.Empty);
        Assert.That (context.Parameters, Is.EqualTo (_mutableCtor.ParameterExpressions));
        Assert.That (context.DeclaringType, Is.SameAs (_declaringType));
        Assert.That (context.IsStatic, Is.False);
        Assert.That (context.PreviousBody, Is.SameAs (_mutableCtor.Body));

        return fakeBody;
      };

      _mutableCtor.SetBody (bodyProvider);

      var expectedBody = Expression.Block (typeof (void), fakeBody);
      ExpressionTreeComparer.CheckAreEqualTrees (expectedBody, _mutableCtor.Body);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "The body of the existing inaccessible constructor 'Void .ctor(Int32)' cannot be replaced.")]
    public void SetBody_NonSettableCtor ()
    {
      var inaccessibleCtor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType (7));
      var descriptor = UnderlyingConstructorInfoDescriptorObjectMother.CreateForExisting (inaccessibleCtor);
      var mutableCtor = Create (descriptor);

      Func<ConstructorBodyModificationContext, Expression> bodyProvider = context =>
      {
        Assert.Fail ("Should not be called.");
        throw new NotImplementedException ();
      };

      mutableCtor.SetBody (bodyProvider);
    }

    [Test]
    public void ToString_WithParameters ()
    {
      var ctorInfo = CreateWithParameters (
          UnderlyingParameterInfoDescriptorObjectMother.CreateForNew (typeof (int), "p1"),
          UnderlyingParameterInfoDescriptorObjectMother.CreateForNew (typeof (string).MakeByRefType (), "p2", ParameterAttributes.Out));

      Assert.That (ctorInfo.ToString (), Is.EqualTo ("Void .ctor(Int32, String&)"));
    }

    [Test]
    public void ToDebugString ()
    {
      var declaringType = MutableTypeObjectMother.CreateForExistingType (GetType());
      var ctorInfo = MutableConstructorInfoObjectMother.CreateForNewWithParameters (
          declaringType,
          UnderlyingParameterInfoDescriptorObjectMother.CreateForNew (typeof (int), "p1"));

      var expected = "MutableConstructor = \"Void .ctor(Int32)\", DeclaringType = \"MutableConstructorInfoTest\"";
      Assert.That (ctorInfo.ToDebugString (), Is.EqualTo (expected));
    }

    [Test]
    public void GetParameters ()
    {
      var parameters = UnderlyingParameterInfoDescriptorObjectMother.CreateMultiple (2);
      var ctorInfo = CreateWithParameters (parameters);

      var result = ctorInfo.GetParameters();

      var actualParameterInfos = result.Select (pi => new { pi.Member, pi.Position, pi.ParameterType, pi.Name, pi.Attributes }).ToArray ();
      var expectedParameterInfos =
          new[]
          {
              new { Member = (MemberInfo) ctorInfo, Position = 0, ParameterType = parameters[0].Type, parameters[0].Name, parameters[0].Attributes },
              new { Member = (MemberInfo) ctorInfo, Position = 1, ParameterType = parameters[1].Type, parameters[1].Name, parameters[1].Attributes },
          };
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
    }

    [Test]
    public void GetParameters_ReturnsSameParameterInfoInstances()
    {
      var ctorInfo = CreateWithParameters (UnderlyingParameterInfoDescriptorObjectMother.CreateForNew());

      var result1 = ctorInfo.GetParameters().Single();
      var result2 = ctorInfo.GetParameters().Single();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetParameters_DoesNotAllowModificationOfInternalList ()
    {
      var ctorInfo = CreateWithParameters (UnderlyingParameterInfoDescriptorObjectMother.CreateForNew ());

      var parameters = ctorInfo.GetParameters ();
      Assert.That (parameters[0], Is.Not.Null);
      parameters[0] = null;

      var parametersAgain = ctorInfo.GetParameters ();
      Assert.That (parametersAgain[0], Is.Not.Null);
    }

    private MutableConstructorInfo Create (UnderlyingConstructorInfoDescriptor underlyingConstructorInfoDescriptor)
    {
      return new MutableConstructorInfo (_declaringType, underlyingConstructorInfoDescriptor);
    }

    private MutableConstructorInfo CreateWithParameters (params UnderlyingParameterInfoDescriptor[] parameterDescriptors)
    {
      return Create (UnderlyingConstructorInfoDescriptorObjectMother.CreateForNew (parameterDescriptors: parameterDescriptors));
    }

    public class DomainType
    {
      public DomainType () { }
      internal DomainType (int i) { Dev.Null = i; }
    }
  }
}