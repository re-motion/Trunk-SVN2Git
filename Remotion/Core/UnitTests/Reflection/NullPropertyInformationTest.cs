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

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class NullPropertyInformationTest
  {
    private NullPropertyInformation _nullPropertyInformation;

    [SetUp]
    public void SetUp ()
    {
      _nullPropertyInformation = new NullPropertyInformation();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_nullPropertyInformation.Name, Is.Null);
      Assert.That (_nullPropertyInformation.DeclaringType, Is.Null);
      Assert.That (_nullPropertyInformation.PropertyType, Is.Null);
      Assert.That (_nullPropertyInformation.CanBeSetFromOutside, Is.False);
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_nullPropertyInformation.GetOriginalDeclaringType(), Is.Null);
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That (_nullPropertyInformation.GetCustomAttribute<ThreadStaticAttribute> (false), Is.Null);
      Assert.That (_nullPropertyInformation.GetCustomAttribute<ThreadStaticAttribute> (true), Is.Null);
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (_nullPropertyInformation.GetCustomAttributes<ThreadStaticAttribute> (false), Is.Empty);
      Assert.That (_nullPropertyInformation.GetCustomAttributes<ThreadStaticAttribute> (true), Is.Empty);
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (_nullPropertyInformation.IsDefined<ThreadStaticAttribute> (false), Is.False);
      Assert.That (_nullPropertyInformation.IsDefined<ThreadStaticAttribute> (true), Is.False);
    }

    [Test]
    public void GetValue ()
    {
      Assert.That (_nullPropertyInformation.GetValue (new object(), null), Is.Null);
    }

    [Test]
    public void GetGetMethod ()
    {
      var result = _nullPropertyInformation.GetGetMethod (false);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (NullMethodInformation)));
    }

    [Test]
    public void GetSetMethod ()
    {
      var result = _nullPropertyInformation.GetSetMethod (false);

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.TypeOf (typeof (NullMethodInformation)));
    }

    [Test]
    public void TestEquals ()
    {
      var nullPropertyInformation2 = new NullPropertyInformation ();

      Assert.That (_nullPropertyInformation.Equals (nullPropertyInformation2), Is.True);
      Assert.That (_nullPropertyInformation.Equals (null), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      Assert.That (_nullPropertyInformation.GetHashCode (), Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "FindInterfaceImplementation can only be called on inteface properties.")]
    public void FindInterfaceImplementation ()
    {
       _nullPropertyInformation.FindInterfaceImplementation (typeof (object));
    }

    [Test]
    public void FindInterfaceDeclaration ()
    {
      Assert.That (_nullPropertyInformation.FindInterfaceDeclaration(), Is.Null);
    }

  }
}