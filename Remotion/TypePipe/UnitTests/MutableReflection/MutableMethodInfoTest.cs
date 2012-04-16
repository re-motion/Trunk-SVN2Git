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
using NUnit.Framework;
using Remotion.TypePipe.MutableReflection;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableMethodInfoTest
  {
    private MutableType _declaringType;
    private UnderlyingMethodInfoDescriptor _descriptor;

    private MutableMethodInfo _methodInfo;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = MutableTypeObjectMother.Create();

      _descriptor = UnderlyingMethodInfoDescriptorObjectMother.CreateForNew ();
      _methodInfo = Create(_descriptor);
    }

    [Test]
    public void Initialization ()
    {
      var mutableMethodInfo = new MutableMethodInfo (_declaringType, _descriptor);

      Assert.That (mutableMethodInfo.DeclaringType, Is.SameAs (_declaringType));
    }

    [Test]
    [Ignore ("TODO 4772")]
    public void UnderlyingSystemMethodInfo ()
    {
      var descriptor = UnderlyingMethodInfoDescriptorObjectMother.CreateForExisting ();
      Assert.That (descriptor.UnderlyingSystemMethodInfo, Is.Not.Null);

      var methodInfo = Create (descriptor);

      Assert.That (methodInfo.UnderlyingSystemMethodInfo, Is.SameAs (descriptor.UnderlyingSystemMethodInfo));
    }

    [Test]
    public void UnderlyingSystemMethodInfo_ForNull ()
    {
      var descriptor = UnderlyingMethodInfoDescriptorObjectMother.CreateForNew ();
      Assert.That (descriptor.UnderlyingSystemMethodInfo, Is.Null);

      var methodInfo = Create (descriptor);

      Assert.That (methodInfo.UnderlyingSystemMethodInfo, Is.SameAs (methodInfo));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_methodInfo.Name, Is.EqualTo (_descriptor.Name));
      }

    [Test]
    public void Attributes ()
    {
      Assert.That (_methodInfo.Attributes, Is.EqualTo (_descriptor.Attributes));
}

    [Test]
    public void ReturnType ()
    {
      Assert.That (_methodInfo.ReturnType, Is.SameAs (_descriptor.ReturnType));
}

    [Test]
    public void Body ()
    {
      Assert.That (_methodInfo.Body, Is.SameAs (_descriptor.Body));
    }

    [Test]
    public void CallingConvention ()
    {
      var instanceDescriptor = UnderlyingMethodInfoDescriptorObjectMother.CreateForNew (attributes: 0);
      var staticDescriptor = UnderlyingMethodInfoDescriptorObjectMother.CreateForNew (attributes: MethodAttributes.Static);

      var instanceMethod = new MutableMethodInfo (_declaringType, instanceDescriptor);
      var staticMethod = new MutableMethodInfo (_declaringType, staticDescriptor);

      Assert.That (instanceMethod.CallingConvention, Is.EqualTo (CallingConventions.HasThis));
      Assert.That (staticMethod.CallingConvention, Is.EqualTo (CallingConventions.Standard));
    }

    [Test]
    public void ParameterExpressions ()
    {
      var parameterDeclarations = ParameterDeclarationObjectMother.CreateMultiple (2);
      var methodInfo = CreateWithParameters (parameterDeclarations);

      Assert.That (methodInfo.ParameterExpressions, Is.EqualTo (parameterDeclarations.Select (pd => pd.Expression)));
    }

    [Test]
    public void GetParameters ()
    {
      var parameter1 = ParameterDeclarationObjectMother.Create();
      var parameter2 = ParameterDeclarationObjectMother.Create ();
      var methodInfo = CreateWithParameters (parameter1, parameter2);

      var result = methodInfo.GetParameters();

      var actualParameterInfos = result.Select (pi => new { pi.Member, pi.Position, pi.ParameterType, pi.Name, pi.Attributes });
      var expectedParameterInfos =
          new[]
          {
              new { Member = (MemberInfo) methodInfo, Position = 0, ParameterType = parameter1.Type, parameter1.Name, parameter1.Attributes },
              new { Member = (MemberInfo) methodInfo, Position = 1, ParameterType = parameter2.Type, parameter2.Name, parameter2.Attributes },
          };
      Assert.That (actualParameterInfos, Is.EqualTo (expectedParameterInfos));
    }

    [Test]
    public void GetParameters_ReturnsSameParameterInfoInstances ()
    {
      var methodInfo = CreateWithParameters (ParameterDeclarationObjectMother.Create ());

      var result1 = methodInfo.GetParameters ().Single ();
      var result2 = methodInfo.GetParameters ().Single ();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetParameters_DoesNotAllowModificationOfInternalList ()
    {
      var methodInfo = CreateWithParameters (ParameterDeclarationObjectMother.CreateMultiple (1));

      var parameters = methodInfo.GetParameters ();
      Assert.That (parameters[0], Is.Not.Null);
      parameters[0] = null;

      var parametersAgain = methodInfo.GetParameters ();
      Assert.That (parametersAgain[0], Is.Not.Null);
    }

    private MutableMethodInfo Create (UnderlyingMethodInfoDescriptor descriptor)
    {
      return new MutableMethodInfo (_declaringType, descriptor);
    }

    private MutableMethodInfo CreateWithParameters (params ParameterDeclaration[] parameterDeclarations)
    {
      var descriptor = UnderlyingMethodInfoDescriptorObjectMother.CreateForNew (parameterDeclarations: parameterDeclarations);
      return new MutableMethodInfo (_declaringType, descriptor);
    }
  }
}