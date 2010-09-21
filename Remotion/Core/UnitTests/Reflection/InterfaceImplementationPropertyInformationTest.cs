// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.UnitTests.Reflection.TestDomain.MemberInfoAdapter;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class InterfaceImplementationPropertyInformationTest
  {
    private IPropertyInformation _implementationPropertyInformationStub;
    private IPropertyInformation _declarationPropertyInformationStub;
    private InterfaceImplementationPropertyInformation _interfaceImplementationPropertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _implementationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      _declarationPropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      _interfaceImplementationPropertyInformation = new InterfaceImplementationPropertyInformation (
          _implementationPropertyInformationStub, _declarationPropertyInformationStub);
    }

    [Test]
    public void Name ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_interfaceImplementationPropertyInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeof(bool));

      Assert.That (_interfaceImplementationPropertyInformation.DeclaringType, Is.SameAs(typeof(bool)));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeof (bool));

      Assert.That (_interfaceImplementationPropertyInformation.GetOriginalDeclaringType(), Is.SameAs (typeof (bool)));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttribute<string>(false)).Return ("Test");

      Assert.That (_interfaceImplementationPropertyInformation.GetCustomAttribute<string> (false), Is.EqualTo("Test"));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new string[0];
      _implementationPropertyInformationStub.Stub (stub => stub.GetCustomAttributes<string> (false)).Return (objToReturn);

      Assert.That (_interfaceImplementationPropertyInformation.GetCustomAttributes<string> (false), Is.SameAs(objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.IsDefined<Attribute>(false)).Return (false);

      Assert.That (_interfaceImplementationPropertyInformation.IsDefined<Attribute>(false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var propertyInfoAdapter = new PropertyInfoAdapter(typeof(string).GetProperty("Length"));
      _implementationPropertyInformationStub.Stub (stub => stub.FindInterfaceImplementation(typeof(bool))).Return (propertyInfoAdapter);

      Assert.That (_interfaceImplementationPropertyInformation.FindInterfaceImplementation (typeof (bool)), Is.SameAs (propertyInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_interfaceImplementationPropertyInformation.FindInterfaceDeclaration(), Is.SameAs (_declarationPropertyInformationStub));
    }

    [Test]
    public void GetIndexParameters ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.GetIndexParameters()).Return (new ParameterInfo[0]);

      Assert.That (_interfaceImplementationPropertyInformation.GetIndexParameters().Length, Is.EqualTo (0));
    }

    [Test]
    public void PropertyType ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof(bool));

      Assert.That (_interfaceImplementationPropertyInformation.PropertyType, Is.SameAs(typeof(bool)));
    }

    [Test]
    public void CanBeSetFromOutside ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.CanBeSetFromOutside).Return (false);

      Assert.That (_interfaceImplementationPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void SetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType> ();
      var value = new SimpleReferenceType ();

      _declarationPropertyInformationStub.Stub (stub => stub.SetValue (instance, value, null)).WhenCalled (
          mi => instance.ImplicitInterfaceScalar = value);

      _interfaceImplementationPropertyInformation.SetValue (instance, value, null);
      Assert.That (instance.ImplicitInterfaceScalar, Is.SameAs (value));
    }

    [Test]
    public void GetValue ()
    {
      var instance = new ClassWithReferenceType<SimpleReferenceType> ();
      var value = new SimpleReferenceType ();

      _declarationPropertyInformationStub.Stub (stub => stub.GetValue (instance, null)).Return (value);

      Assert.That (_interfaceImplementationPropertyInformation.GetValue(instance, null), Is.SameAs (value));
    }

    [Test]
    public void GetGetMethod ()
    {
      var implementationPropertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar");
      var implementationMethodInfoAdapter = new MethodInfoAdapter (implementationPropertyInfo.GetGetMethod ());
      _implementationPropertyInformationStub.Stub (stub => stub.GetGetMethod (true)).Return (implementationMethodInfoAdapter);

      var declarationPropertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar");
      var declarationMethodInfoAdapter = new MethodInfoAdapter (declarationPropertyInfo.GetGetMethod ());
      _declarationPropertyInformationStub.Stub (stub => stub.GetGetMethod (true)).Return (declarationMethodInfoAdapter);

      var result = _interfaceImplementationPropertyInformation.GetGetMethod (true);

      Assert.That (result, Is.TypeOf (typeof (InterfaceImplementationMethodInformation)));
      Assert.That (result.DeclaringType, Is.SameAs(typeof (ClassWithReferenceType<SimpleReferenceType>)));
      Assert.That (result.Name, Is.EqualTo ("get_ImplicitInterfaceScalar"));
    }

    [Test]
    public void GetSetMethod ()
    {
      var implementationPropertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar");
      var implementationMethodInfoAdapter = new MethodInfoAdapter (implementationPropertyInfo.GetSetMethod ());
      _implementationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (implementationMethodInfoAdapter);

      var declarationPropertyInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetProperty ("ImplicitInterfaceScalar");
      var declarationMethodInfoAdapter = new MethodInfoAdapter (declarationPropertyInfo.GetSetMethod ());
      _declarationPropertyInformationStub.Stub (stub => stub.GetSetMethod (true)).Return (declarationMethodInfoAdapter);

      var result = _interfaceImplementationPropertyInformation.GetSetMethod (true);

      Assert.That (result, Is.TypeOf (typeof (InterfaceImplementationMethodInformation)));
      Assert.That (result.DeclaringType, Is.SameAs (typeof (ClassWithReferenceType<SimpleReferenceType>)));
      Assert.That (result.Name, Is.EqualTo ("set_ImplicitInterfaceScalar"));
    }

    [Test]
    public void To_String ()
    {
      _implementationPropertyInformationStub.Stub (stub => stub.Name).Return ("Test");
      _declarationPropertyInformationStub.Stub (stub => stub.DeclaringType).Return (typeof(bool));

      Assert.That (_interfaceImplementationPropertyInformation.ToString (), Is.EqualTo ("Test(impl of 'Boolean'"));
    }

   
  }
}