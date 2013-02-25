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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection.Generics;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.TypePipe.UnitTests.MutableReflection.Implementation;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Development.UnitTesting;

namespace Remotion.TypePipe.UnitTests.MutableReflection.Generics
{
  [TestFixture]
  public class GenericParameterTest
  {
    private const BindingFlags c_allMembers = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private string _name;
    private string _namespace;
    private GenericParameterAttributes _genericParameterAttributes;
    private Type _baseTypeConstraint;
    private Type _interfaceConstraint;
    private IMemberSelector _memberSelectorMock;

    private GenericParameter _parameter;

    [SetUp]
    public void SetUp ()
    {
      _memberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector>();
      _name = "parameter";
      _namespace = "namespace";
      _genericParameterAttributes = (GenericParameterAttributes) 7;
      _baseTypeConstraint = typeof (DomainType);
      _interfaceConstraint = ReflectionObjectMother.GetSomeInterfaceType();

      _parameter = new GenericParameter (
          _memberSelectorMock, _name, _namespace, _genericParameterAttributes, _baseTypeConstraint, new[] { _interfaceConstraint }.AsOneTime ());
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_parameter.Name, Is.EqualTo (_name));
      Assert.That (_parameter.Namespace, Is.EqualTo (_namespace));
      Assert.That (_parameter.FullName, Is.Null);
      Assert.That (
          _parameter.Attributes, Is.EqualTo (TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.Public));
      Assert.That (_parameter.GenericParameterAttributes, Is.EqualTo (_genericParameterAttributes));
      Assert.That (_parameter.BaseType, Is.SameAs (_baseTypeConstraint));

    }

    [Test]
    public void IsGenericParameter ()
    {
      Assert.That (_parameter.IsGenericParameter, Is.True);
    }

    [Test]
    public void GetGenericParameterConstraints ()
    {
      var result = _parameter.GetGenericParameterConstraints();

      Assert.That (result, Is.EqualTo (new[] { _baseTypeConstraint, _interfaceConstraint }));
    }

    [Test]
    public void GetGenericParameterConstraints_NoBaseTypeConstraint ()
    {
      var baseTypeConstraint = typeof (object);
      var parameter = new GenericParameter (
          _memberSelectorMock, _name, _namespace, _genericParameterAttributes, baseTypeConstraint, new[] { _interfaceConstraint });

      var result = parameter.GetGenericParameterConstraints();

      Assert.That (result, Is.EqualTo (new[] { _interfaceConstraint }));
    }

    [Test]
    public void GetAllInterfaces ()
    {
      var result = _parameter.InvokeNonPublicMethod ("GetAllInterfaces");

      Assert.That (result, Is.EqualTo (new[] { typeof (IDomainInterface), _interfaceConstraint }));
    }

    [Test]
    public void GetAllFields ()
    {
      var result = _parameter.InvokeNonPublicMethod ("GetAllFields");

      var field = NormalizingMemberInfoFromExpressionUtility.GetField ((DomainType obj) => obj.Field);
      Assert.That (result, Has.Member (field));
      Assert.That (result, Is.EquivalentTo (typeof (DomainType).GetFields (c_allMembers)));
    }

    [Test]
    public void GetAllConstructors ()
    {
      Assert.That (typeof (DomainType).GetConstructors(), Is.Not.Empty);
      var result = _parameter.InvokeNonPublicMethod ("GetAllConstructors");

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetAllMethods ()
    {
      var result = _parameter.InvokeNonPublicMethod ("GetAllMethods");

      var method = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.Method());
      Assert.That (result, Has.Member (method));
      Assert.That (result, Is.EquivalentTo (typeof (DomainType).GetMethods (c_allMembers)));
    }

    [Test]
    public void GetAllProperties ()
    {
      var result = _parameter.InvokeNonPublicMethod<IEnumerable<PropertyInfo>> ("GetAllProperties");

      var property = NormalizingMemberInfoFromExpressionUtility.GetProperty ((DomainType obj) => obj.Property);
      Assert.That (result, Is.EqualTo (new[] { property }));
    }

    [Test]
    public void GetAllEvents ()
    {
      var result = _parameter.InvokeNonPublicMethod<IEnumerable<EventInfo>> ("GetAllEvents");

      var event_ = typeof (DomainType).GetEvent ("Event");
      Assert.That (result, Is.EqualTo (new[] { event_ }));
    }

    [Test]
    public void GetAllXXX_UsesAllBindingFlagsToRetrieveMembers ()
    {
      var fields = _baseTypeConstraint.GetFields (c_allMembers);
      var methods = _baseTypeConstraint.GetMethods (c_allMembers);
      var properties = _baseTypeConstraint.GetProperties (c_allMembers);
      var events = _baseTypeConstraint.GetEvents (c_allMembers);

      var baseMemberSelectorMock = MockRepository.GenerateStrictMock<IMemberSelector>();
      var baseTypeConstraint = CustomTypeObjectMother.Create (
          baseMemberSelectorMock, fields: fields, methods: methods, properties: properties, events: events);

      baseMemberSelectorMock.Expect (mock => mock.SelectFields (fields, c_allMembers, baseTypeConstraint)).Return (fields);
      baseMemberSelectorMock.Expect (mock => mock.SelectMethods (methods, c_allMembers, baseTypeConstraint)).Return (methods);
      baseMemberSelectorMock.Expect (mock => mock.SelectProperties (properties, c_allMembers, baseTypeConstraint)).Return (properties);
      baseMemberSelectorMock.Expect (mock => mock.SelectEvents (events, c_allMembers, baseTypeConstraint)).Return (events);

      var parameter = new GenericParameter (
          _memberSelectorMock, _name, _namespace, _genericParameterAttributes, baseTypeConstraint, new[] { _interfaceConstraint }.AsOneTime());

      parameter.InvokeNonPublicMethod ("GetAllFields");
      parameter.InvokeNonPublicMethod ("GetAllConstructors");
      parameter.InvokeNonPublicMethod ("GetAllMethods");
      parameter.InvokeNonPublicMethod ("GetAllProperties");
      parameter.InvokeNonPublicMethod ("GetAllEvents");

      baseMemberSelectorMock.AssertWasNotCalled (
          mock => mock.SelectMethods (Arg<IEnumerable<ConstructorInfo>>.Is.Anything, Arg<BindingFlags>.Is.Anything, Arg<Type>.Is.Anything));
      baseMemberSelectorMock.VerifyAllExpectations();
    }

    class DomainType : IDomainInterface
    {
      public int Field;
      public void Method () { }
      public int Property { get; set; }
      public event EventHandler Event;
    }

    interface IDomainInterface { }
  }
}