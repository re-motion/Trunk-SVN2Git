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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Generics
{
  [TestFixture]
  public class ConstructorOnTypeInstantiationTest
  {
    private TypeInstantiation _declaringType;
    private ITypeAdjuster _typeAdjusterMock;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = TypeInstantiationObjectMother.Create();
      _typeAdjusterMock = MockRepository.GenerateStrictMock<ITypeAdjuster> ();
    }

    [Test]
    public void Initialization ()
    {
      var ctor = NormalizingMemberInfoFromExpressionUtility.GetConstructor (() => new DomainType (7));
      var fakeType = ReflectionObjectMother.GetSomeType();
      _typeAdjusterMock
          .Expect (mock => mock.SubstituteGenericParameters (ctor.GetParameters().Single().ParameterType))
          .Return (fakeType);
      var result = new ConstructorOnTypeInstantiation (_declaringType, _typeAdjusterMock, ctor);

      _typeAdjusterMock.VerifyAllExpectations ();
      Assert.That (result.DeclaringType, Is.SameAs (_declaringType));
      Assert.That (result.Attributes, Is.EqualTo (ctor.Attributes));
      Assert.That (result.ConstructorOnGenericType, Is.SameAs (ctor));

      var parameter = result.GetParameters().Single();
      Assert.That (parameter, Is.TypeOf<MemberParameterOnTypeInstantiation> ());
      Assert.That (parameter.Member, Is.SameAs (result));
      Assert.That (parameter.As<MemberParameterOnTypeInstantiation>().MemberParameterOnGenericType, Is.EqualTo (ctor.GetParameters().Single()));
    }

    private class DomainType
    {
      public DomainType (int i) { Dev.Null = i; }
    }
  }
}