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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Implementation;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class ConcreteImplementationAttributeTest
  {
    private ConcreteImplementationAttribute _attribute;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new ConcreteImplementationAttribute ("Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, "
          + "Remotion.UnitTests, Version = <version>");
    }

    [Test]
    public void ResolveType ()
    {
      FrameworkVersion.Value = typeof (ConcreteImplementationAttributeTest).Assembly.GetName ().Version;
      
      var result = _attribute.ResolveType();

      Assert.That (result, Is.Not.Null);
      Assert.That (result, Is.EqualTo(typeof (ConcreteImplementationAttributeTest)));
    }

    [Test]
    public void InstantiateType ()
    {
      var instance = _attribute.InstantiateType (typeof (ConcreteImplementationAttributeTest));
      Assert.IsNotNull (instance);
      Assert.IsInstanceOfType (typeof (ConcreteImplementationAttributeTest), instance);
    }

    [Test]
    public void InstantiateType_WithArguments ()
    {
      var instance = _attribute.InstantiateType (typeof (StringBuilder), "test");
      Assert.IsNotNull (instance);
      Assert.That (instance, Is.TypeOf(typeof(StringBuilder)));
      Assert.That (instance.ToString(), Is.EqualTo("test"));
    }

  }
}
