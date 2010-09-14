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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class MixinIntroducedMethodInformationTest
  {
    private IMethodInformation _mixinMethodInformationStub;
    private MixinIntroducedMethodInformation _mixinIntroducedMethodInformation;

    [SetUp]
    public void SetUp ()
    {
      _mixinMethodInformationStub = MockRepository.GenerateStub<IMethodInformation>();
      _mixinIntroducedMethodInformation = new MixinIntroducedMethodInformation (_mixinMethodInformationStub);
    }

    [Test]
    public void Name ()
    {
      _mixinMethodInformationStub.Stub (stub => stub.Name).Return ("Test");

      Assert.That (_mixinIntroducedMethodInformation.Name, Is.EqualTo ("Test"));
    }

    [Test]
    public void DeclaringType ()
    {
      _mixinMethodInformationStub.Stub (stub => stub.DeclaringType).Return (typeof(object));

      Assert.That (_mixinIntroducedMethodInformation.DeclaringType, Is.SameAs(typeof(object)));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      _mixinMethodInformationStub.Stub (stub => stub.GetOriginalDeclaringType()).Return (typeof (object));

      Assert.That (_mixinIntroducedMethodInformation.GetOriginalDeclaringType(), Is.SameAs (typeof (object)));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      var objToReturn = new object();
      _mixinMethodInformationStub.Stub (stub => stub.GetCustomAttribute<object> (false)).Return (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetCustomAttribute<object>(false), Is.SameAs (objToReturn));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      var objToReturn = new object[0];
      _mixinMethodInformationStub.Stub (stub => stub.GetCustomAttributes<object> (false)).Return (objToReturn);

      Assert.That (_mixinIntroducedMethodInformation.GetCustomAttributes<object> (false), Is.SameAs (objToReturn));
    }

    [Test]
    public void IsDefined ()
    {
      _mixinMethodInformationStub.Stub (stub => stub.IsDefined<object>(false)).Return (false);

      Assert.That (_mixinIntroducedMethodInformation.IsDefined<object>(false), Is.False);
    }

    [Test]
    public void FindInterfaceImplementation ()
    {
      var methodInfoAdapter = new MethodInfoAdapter (typeof (object).GetMethod ("ToString"));
      _mixinMethodInformationStub.Stub (stub => stub.FindInterfaceImplementation (typeof (object))).Return (methodInfoAdapter);

      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceImplementation (typeof (object)), Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      var methodInfoAdapter = new MethodInfoAdapter (typeof (object).GetMethod ("ToString"));
      _mixinMethodInformationStub.Stub (stub => stub.FindInterfaceDeclaration ()).Return (methodInfoAdapter);

      Assert.That (_mixinIntroducedMethodInformation.FindInterfaceDeclaration (), Is.SameAs (methodInfoAdapter));
    }

    [Test]
    public void FindDeclaringProperty ()
    {
      var propertyInfoAdapter = new PropertyInfoAdapter (typeof (string).GetProperty ("Length"));
      _mixinMethodInformationStub.Stub (stub => stub.FindDeclaringProperty (typeof (string))).Return (propertyInfoAdapter);

      Assert.That (_mixinIntroducedMethodInformation.FindDeclaringProperty (typeof (string)), Is.SameAs (propertyInfoAdapter));
    }

    [Test]
    public void ReturnType ()
    {
      _mixinMethodInformationStub.Stub (stub => stub.ReturnType).Return (typeof (object));

      Assert.That (_mixinIntroducedMethodInformation.ReturnType, Is.SameAs (typeof (object)));
    }

    [Test]
    public void Invoke ()
    {
      var methodInfoAdapter = new MethodInfoAdapter (typeof (object).GetMethod ("ToString"));
      _mixinMethodInformationStub.Stub (stub => stub.FindInterfaceDeclaration ()).Return (methodInfoAdapter);

      var result = _mixinIntroducedMethodInformation.Invoke ("Test", new object[] { });

      Assert.That (result, Is.EqualTo ("Test"));
    }

  }
}