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
using Remotion.Implementation;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class ConcreteImplementationAttributeTest
  {
    private ConcreteImplementationAttribute _attribute;
    private string _typeNameTemplate;

    [SetUp]
    public void SetUp ()
    {
      _typeNameTemplate = "Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, Remotion.UnitTests, Version = <version>";
      _attribute = new ConcreteImplementationAttribute (_typeNameTemplate);
      _attribute.Lifetime = LifetimeKind.Singleton;
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_attribute.TypeNameTemplate, Is.SameAs (_typeNameTemplate));
      Assert.That (_attribute.Lifetime, Is.EqualTo (LifetimeKind.Singleton));
    }

    [Test]
    public void InitializationWithTyp ()
    {
      _attribute = new ConcreteImplementationAttribute (typeof (ConcreteImplementationAttributeTest));

      Assert.That (
        _attribute.TypeNameTemplate.StartsWith ("Remotion.UnitTests.Interfaces.Implementation.ConcreteImplementationAttributeTest, Remotion.UnitTests"), 
        Is.True);
    }
  }
}
